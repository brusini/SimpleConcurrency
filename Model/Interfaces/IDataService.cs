using System;
using System.Collections.Generic;

namespace SimpleConcurrency.Model.Interfaces
{
    public interface IDataService
    {
        string ConnectionString { get; set; }
        List<YUser> GetUsersY();
        List<XUser> GetUsersX();
        XUser GetUserX(int id);
        YUser GetYUserForEditing(int id, string ownerId);
        int AddUsersY(string firstName, string lastName, bool isBlocked, string ownerId);
        int AddUsersX(string firstName, string lastName, bool isDeleted, DateTime updateDate);
        int DeleteXUser(int id, DateTime updateDate);
        int DeleteYUser(int id, string ownerId);
        int UpdateXUser(int id, string firstName, string lastName, DateTime updateDate, DateTime newUpdateDate);
        int UpdateYUser(int id, string firstName, string lastName, string ownerId);
        int ReleaseYUser(int id, string ownerId);
    }
}