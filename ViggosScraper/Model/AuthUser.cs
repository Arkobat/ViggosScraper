using DrikDatoApp.Model;
using ViggosScraper.Database;

namespace ViggosScraper.Model;

public record AuthUser(SelfUserDto SelfUser, DbUser User);