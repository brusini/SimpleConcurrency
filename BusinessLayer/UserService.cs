using System;
using System.Collections.Generic;
using System.Configuration;
using BusinessLayer.Interfaces;
using SimpleConcurrency.DataLayer;
using SimpleConcurrency.Model;
using SimpleConcurrency.Model.Interfaces;

namespace SimpleConcurrency.BusinessLayer
{
    public class UserService : IBusinessService
    {
        private readonly IDataService _dataService;
        public UserService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public UserService()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["defaultConnectionString"];

            _dataService = new UserDataService(connectionString.ConnectionString);
        }

        public List<YUser> GetUsersY()
        {
            return _dataService.GetUsersY();
        }

        public List<XUser> GetUsersX()
        {
            return _dataService.GetUsersX();
        }

        public XUser GetUserX(int id)
        {
            return _dataService.GetUserX(id);
        }

        public YUser GetYUserForEditing(int id, string owner)
        {
            return _dataService.GetYUserForEditing(id, owner);
        }

        public int AddXUser(string firstName, string lastName, bool isBlocked, string ownerId)
        {
            return _dataService.AddUsersY(firstName, lastName, isBlocked, ownerId);
        }

        public int AddYUser(string firstName, string lastName, bool isDeleted, DateTime updateDate)
        {
            return _dataService.AddUsersX(firstName, lastName, isDeleted, updateDate);
        }

        public int DeleteXUser(int id, DateTime updateDate)
        {
            return _dataService.DeleteXUser(id, updateDate);
        }

        public int DeleteYUser(int id, string ownerId)
        {
            return _dataService.DeleteYUser(id, ownerId);
        }

        public int UpdateXUser(int id, string firstName, string lastName, DateTime updateDate, DateTime newUpdateDate)
        {
            return _dataService.UpdateXUser(id, firstName, lastName, updateDate, newUpdateDate);
        }

        public int UpdateYUser(int id, string firstName, string lastName, string ownerId)
        {
            return _dataService.UpdateYUser(id, firstName, lastName, ownerId);
        }

        public int ReleaseYUser(int id, string ownerId)
        {
            return _dataService.ReleaseYUser(id, ownerId);
        }
    }
}