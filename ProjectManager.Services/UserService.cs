using System;
using System.Linq;
using ProjectManager.Contracts;
using ProjectManager.Models;

namespace ProjectManager.Services
{
    public class UserService
    {
        private readonly IRepository<AppUser> _userRepository;

        public UserService(IRepository<AppUser> userRepository)
        {
            this._userRepository = userRepository;
        }

        public bool UserIsValid(string username, string password)
        {
            var user = _userRepository.GetAll()
                .SingleOrDefault(u => u.Username == username);

            return user != null && user.Password == password;
        }

        public AppUser GetUserInfo(string username)
        {
            var user = _userRepository.GetAll()
                .FirstOrDefault(u => u.Username == username);

            return user;
        }

        public bool CreateUser(string username, string password, string email, Role role)
        {
            var existingUser = _userRepository.GetAll()
                .SingleOrDefault(u => u.Username == username);

            if (existingUser != null)
                return false;

            var newUser = new AppUser
            {
                Username = username,
                Password = password,
                Email = email,
                Role = role,
                LastLoginDate = new DateTime(1990, 1, 1)
            };

            _userRepository.Insert(newUser);
            var changes = _userRepository.Commit();

            return (changes > 0);
        }

        public void DeleteUser(string username)
        {
            var user = _userRepository.GetAll()
                .SingleOrDefault(u => u.Username == username);

            if (user == null)
                return;

            _userRepository.Delete(user.Id);
        }

        public Role? GetRole(string username)
        {
            var user = _userRepository.GetAll()
                .SingleOrDefault(u => u.Username == username);

            return user?.Role;
        }

        public void UpdateUser(AppUser updatedUser)
        {
            var currentUser = _userRepository.GetById(updatedUser.Id);

            if (currentUser == null)
                return;

            currentUser.Email = updatedUser.Email;
            currentUser.Password = updatedUser.Password;

            _userRepository.Update(currentUser);
            _userRepository.Commit();
        }

        public void SetLastLogin(string userName, DateTime lastLoginDate)
        {
            var user = _userRepository.GetAll()
                .FirstOrDefault(u => u.Username == userName);

            if (user == null)
                return;

            user.LastLoginDate = lastLoginDate; 

            _userRepository.Update(user);
            _userRepository.Commit();
        }
    }
}
