namespace AndrewAI.Services
{
    using System;
    using AndrewAI.Data;
    using AndrewAI.Entities;

    public interface IUserAuthenticationService
    {
        bool CreateUser(User model);
        User? GetUser(Guid? id, string username);
        bool UserExist(Guid tenantId, string username, string emailId);
        bool CreateRefreshTokenByUser(Guid tenantId, Guid userId, string token);
        bool UpdateRefreshTokenByUser(Guid tenantId, Guid refreshTokenId, Guid userId, string token);
        Guid? RefreshTokenExist(Guid tenantId, string token, Guid userId);
    }

    public class UserAuthenticationService : IUserAuthenticationService
    {
        private AndrewAIContext _dbContext;
        public UserAuthenticationService(AndrewAIContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CreateUser(User model)
        {
            if (model != null)
            {
                model.CreatedOn = DateTime.UtcNow;
                model.UpdatedOn = DateTime.UtcNow;
                _dbContext.User.Add(model);
                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public User? GetUser(Guid? id, string username)
        {
            if (id == null && !string.IsNullOrEmpty(username))
                return _dbContext.User.FirstOrDefault(u => u.UserName == username);
            return _dbContext.User.FirstOrDefault(u => u.Id == id);
        }

        public bool UserExist(Guid tenantId, string username, string emailId)
        {
            return _dbContext.User.Any(u => u.EmailId == emailId || u.UserName == username && u.TenantId == tenantId);
        }

        public bool UpdateRefreshTokenByUser(Guid tenantId, Guid refreshTokenId, Guid userId, string token)
        {
            UserToken model = new UserToken
            {
                UserId = userId,
                TenantId = tenantId,
                RefershToken = token,
                CreatedOn = DateTime.UtcNow
            };
            _dbContext.UserToken.Update(model);
            _dbContext.SaveChanges();
            return true;
        }

        public bool CreateRefreshTokenByUser(Guid tenantId, Guid userId, string token)
        {
            UserToken model = new UserToken
            {
                Id = new Guid(),
                UserId = userId,
                TenantId = tenantId,
                RefershToken = token,
                CreatedOn = DateTime.UtcNow
            };
            _dbContext.UserToken.Add(model);
            _dbContext.SaveChanges();
            return true;
        }

        public Guid? RefreshTokenExist(Guid tenantId, string token, Guid userId)
        {
            return _dbContext.UserToken.FirstOrDefault(u => u.TenantId == tenantId && u.RefershToken == token && u.UserId == userId)?.Id;
        }
    }
}