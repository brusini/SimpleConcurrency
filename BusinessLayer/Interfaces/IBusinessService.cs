using System;
using System.Collections.Generic;
using SimpleConcurrency.Model;

namespace BusinessLayer.Interfaces
{
    public interface IBusinessService
    {
        List<YUser> GetUsersY();
        List<XUser> GetUsersX();
        XUser GetUserX(int id);
        YUser GetYUserForEditing(int id, string owner);
        int AddXUser(string firstName, string lastName, bool isBlocked, string ownerId);
        int AddYUser(string firstName, string lastName, bool isDeleted, DateTime updateDate);
        int DeleteXUser(int id, DateTime updateDate);
        int DeleteYUser(int id, string ownerId);
        int UpdateXUser(int id, string firstName, string lastName, DateTime updateDate, DateTime newUpdateDate);
        int UpdateYUser(int id, string firstName, string lastName, string ownerId);
        int ReleaseYUser(int id, string ownerId);
    }
}
