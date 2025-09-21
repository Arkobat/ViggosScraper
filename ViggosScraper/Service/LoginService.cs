using DrikDatoApp.Model;
using DrikDatoApp.Service;
using Microsoft.EntityFrameworkCore;
using ViggosScraper.Database;
using ViggosScraper.Model;

namespace ViggosScraper.Service;

public class LoginService(
    IDrikDatoService drikDatoService,
    UserScraper userScraper,
    ViggosDb dbContext,
    ICache<string, AuthUser> loginCache
)
{
    public async Task<AuthUser> Login(string phoneNumber, string password)
    {
        var response = await drikDatoService.Login(phoneNumber, password);
        if (response.Status != 1)
        {
            // TODO: Handle invalid login attempts
            throw new UnauthorizedAccessException("Invalid login credentials.");
        }

        return await CacheLoginResponse(response.Player!);
    }

    public async Task<AuthUser> ValidateToken(string token)
    {
        if (loginCache.TryGet(token, out var cachedResponse))
        {
            return cachedResponse;
        }

        var response = await drikDatoService.Authorize(token);
        if (response.Status != 1)
        {
            // TODO: Handle invalid login attempts
            throw new UnauthorizedAccessException("Invalid token.");
        }

        return await CacheLoginResponse(response.Player!);
    }

    private async Task<AuthUser> CacheLoginResponse(SelfUserDto response)
    {
        var dbUser = await dbContext.Users
            .Include(u => u.Datoer)
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.ProfileId == response.Id);

        if (dbUser is null)
        {
            dbUser = await userScraper.CreateUser(response);
        }
        else
        {
            await userScraper.UpdateUser(dbUser, response);
        }

        var authUser = new AuthUser(response, dbUser);
        loginCache.Set(response.Token!, authUser, TimeSpan.FromMinutes(60));
        return authUser;
    }

    public async Task<AvatarChangeResponse> ChangeAvatar(string token, MemoryStream imageStream)
    {
        var authUser = await ValidateToken(token);
        var fileId = await drikDatoService.UploadAvatar(authUser.User.ProfileId.ToString(), imageStream);
        var success = await drikDatoService.SetAvatar(authUser.User.ProfileId.ToString(), fileId, authUser.SelfUser.Token!);
        
        if (success.Success)
        {
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.ProfileId == authUser.User.ProfileId);
            if (dbUser != null)
            {
                dbUser.AvatarUrl = fileId;
                await dbContext.SaveChangesAsync();
                authUser.User.AvatarUrl = fileId;
                authUser.SelfUser.Photo = fileId;
            }
        }

        return success;
    }
}