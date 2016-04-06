using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SimpleConcurrency.DataLayer.Properties;
using SimpleConcurrency.Model;
using SimpleConcurrency.Model.Interfaces;

namespace SimpleConcurrency.DataLayer
{
    public class UserDataService : IDataService
    {
        public string ConnectionString { get; set; }

        public UserDataService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<YUser> GetUsersY()
        {
            var result = new List<YUser>();
            using (var connection = new SqlConnection(ConnectionString))
            {

                #region SQL
                /*SELECT * FROM YUsers WHERE IsDeleted = 0*/
                #endregion
                using (var command = new SqlCommand(Settings.Default.GetAllYUsersCmd, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var u = new YUser
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                IsBlocked = reader.GetBoolean(3),
                                OwnerId = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                            };

                            result.Add(u);
                        }
                    }
                }
            }

            return result;
        }

        public List<XUser> GetUsersX()
        {
            var result = new List<XUser>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*SELECT * FROM XUsers WHERE IsDeleted = 0*/
                #endregion
                using (var command = new SqlCommand(Settings.Default.GetAllXUsersCmd, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var u = new XUser
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                IsDeleted = reader.GetBoolean(3),
                                UpdateDate = reader.GetDateTime(4)
                            };

                            result.Add(u);
                        }
                    }
                }
            }

            return result;
        }

        public XUser GetUserX(int id)
        {
            var result = new XUser();
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*SELECT * FROM XUsers WHERE Id=@Id AND IsDeleted = 0*/
                #endregion
                using (var command = new SqlCommand(Settings.Default.GetXUserCmd, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    connection.Open();
                    command.ExecuteNonQuery();

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result = new XUser
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                IsDeleted = reader.GetBoolean(3),
                                UpdateDate = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }

            return result;
        }

        public YUser GetYUserForEditing(int id, string owner)
        {
            var result = new YUser();
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*
                CREATE PROCEDURE [dbo].[GetYUserForEditing]
	                @id int,
	                @owner nvarchar(50)
                AS
	                SET NOCOUNT, XACT_ABORT ON;

	                BEGIN TRY
		                BEGIN TRANSACTION
	
		                IF EXISTS(SELECT * FROM YUsers WHERE Id = @id AND IsBlocked = 1 AND OwnerId != @owner) BEGIN
			                ROLLBACK TRANSACTION
			                RETURN -1
		                END

		                UPDATE YUsers 
		                SET	   IsBlocked = 1, 
			                   OwnerId = @owner
		                WHERE Id = @id

		                SELECT * FROM YUsers WHERE Id = @id
	
	                COMMIT TRANSACTION
	                END TRY
	                BEGIN CATCH
		                IF XACT_STATE() <> 0 
		                ROLLBACK TRANSACTION
	                RAISERROR ('Error in executing transaction', 16, 1)
	                END CATCH
                */
                #endregion
                using (var command = new SqlCommand(Settings.Default.GetYUserForEditingSPName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("owner", owner);

                    var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        reader.Read();

                        var ret = returnParameter.Value;
                        if (ret != null && (int)ret == -1)
                        {
                            result.IsBlocked = true;
                        }
                        else
                        {
                            result = new YUser
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                IsBlocked = false,
                                OwnerId = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                            };
                        }
                    }
                }
            }

            return result;
        }

        public int AddUsersY(string firstName, string lastName, bool isBlocked, string ownerId)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*insert into YUsers (FirstName, LastName) values(@FirstName, @LastName); SELECT SCOPE_IDENTITY()*/
                #endregion
                using (var command = new SqlCommand(Settings.Default.AddYUserCmd, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("FirstName", firstName);
                    command.Parameters.AddWithValue("LastName", lastName);

                    result = (int)(decimal)command.ExecuteScalar();
                }
            }

            return result;
        }

        public int AddUsersX(string firstName, string lastName, bool isDeleted, DateTime updateDate)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*insert into XUsers (FirstName, LastName, IsDeleted, UpdateDate) values(@FirstName, @LastName, @IsDeleted, @UpdateDate); SELECT SCOPE_IDENTITY()*/
                #endregion
                using (var command = new SqlCommand(Settings.Default.AddXUserCmd, connection))
                {
                    connection.Open();

                    command.Parameters.AddWithValue("FirstName", firstName);
                    command.Parameters.AddWithValue("LastName", lastName);
                    command.Parameters.AddWithValue("IsDeleted", isDeleted);
                    command.Parameters.AddWithValue("UpdateDate", updateDate);

                    result = (int)(decimal)command.ExecuteScalar();
                }
            }

            return result;
        }

        public int DeleteXUser(int id, DateTime updateDate)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*
                CREATE PROCEDURE [dbo].[DeleteXUser]
	                @id int,
	                @updateDate DateTime
                AS
	                SET NOCOUNT, XACT_ABORT ON;

	                BEGIN TRY
		                BEGIN TRANSACTION

		                IF EXISTS(SELECT * FROM XUsers WHERE Id = @id AND (UpdateDate != @updateDate OR IsDeleted = 1)) BEGIN
			                ROLLBACK TRANSACTION
			                RETURN -1
		                END

		                UPDATE XUsers 
		                SET 
			                IsDeleted = 1
		                WHERE Id = @id

		                COMMIT TRANSACTION
	                END TRY
	                BEGIN CATCH
		                IF XACT_STATE() <> 0 
		                ROLLBACK TRANSACTION
	                RAISERROR ('Error in executing transaction', 16, 1)
	                END CATCH
                RETURN 0
                */
                #endregion
                using (var command = new SqlCommand(Settings.Default.DeleteXUserSpName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("updateDate", updateDate);

                    var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();

                    command.ExecuteNonQuery();

                    result = (int)returnParameter.Value;
                }
            }

            return result;
        }

        public int DeleteYUser(int id, string ownerId)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*
                CREATE PROCEDURE [dbo].[DeleteYUser]
	                @id int,
	                @ownerId nvarchar(50)
                AS
	                SET NOCOUNT, XACT_ABORT ON;

	                BEGIN TRY
		                BEGIN TRANSACTION
		                IF EXISTS(SELECT * FROM YUsers WHERE Id = @id AND ((IsBlocked = 1 AND OwnerId != @ownerId) OR IsDeleted = 1)) BEGIN
			                ROLLBACK TRANSACTION
			                RETURN -1
		                END

		                UPDATE YUsers 
		                SET 
			                IsDeleted = 1
		                WHERE Id = @id

	                COMMIT TRANSACTION
	                END TRY
	                BEGIN CATCH
		                IF XACT_STATE() <> 0 
		                ROLLBACK TRANSACTION
	                RAISERROR ('Error in executing transaction', 16, 1)
	                END CATCH
                RETURN 0
                */
                #endregion
                using (var command = new SqlCommand(Settings.Default.DeleteYUserSPName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("ownerId", ownerId);

                    var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();

                    command.ExecuteNonQuery();

                    result = (int)returnParameter.Value;
                }
            }

            return result;
        }

        public int UpdateXUser(int id, string firstName, string lastName, DateTime updateDate, DateTime newUpdateDate)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*
                CREATE PROCEDURE [dbo].[UpdateXUser]
	                @id int,
	                @firstName nvarchar(50),
	                @lastName nvarchar(50),
	                @updateDate DateTime,
	                @newUpdateDate DateTime
                AS
	                SET NOCOUNT, XACT_ABORT ON;

	                BEGIN TRY
		                BEGIN TRANSACTION

		                IF EXISTS(SELECT * FROM XUsers WHERE Id = @id AND IsDeleted = 1) BEGIN
			                ROLLBACK TRANSACTION
			                RETURN -1
		                END
		                IF EXISTS(SELECT * FROM XUsers WHERE Id = @id AND IsDeleted = 0 AND UpdateDate != @updateDate) BEGIN
			                ROLLBACK TRANSACTION
			                RETURN -2
		                END

		                UPDATE XUsers 
		                SET 
			                FirstName = @firstName, 
			                LastName = @lastName,
			                UpdateDate = @newUpdateDate
		                WHERE Id = @id

		                COMMIT TRANSACTION
	                END TRY
	                BEGIN CATCH
		                IF XACT_STATE() <> 0 
		                ROLLBACK TRANSACTION
	                RAISERROR ('Error in executing transaction', 16, 1)
	                END CATCH
                RETURN 0
                */
                #endregion
                using (var command = new SqlCommand(Settings.Default.UpdateXUserSPName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("firstName", firstName);
                    command.Parameters.AddWithValue("lastName", lastName);
                    command.Parameters.AddWithValue("updateDate", updateDate);
                    command.Parameters.AddWithValue("newUpdateDate", newUpdateDate);

                    var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();

                    command.ExecuteNonQuery();

                    result = (int)returnParameter.Value;
                }
            }

            return result;
        }

        public int UpdateYUser(int id, string firstName, string lastName, string ownerId)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*
                CREATE PROCEDURE [dbo].[UpdateYUser]
	                @id int,
	                @firstName nvarchar(50),
	                @lastName nvarchar(50),
	                @ownerId nvarchar(50)
                AS
	                SET NOCOUNT, XACT_ABORT ON;

	                BEGIN TRY
		                BEGIN TRANSACTION
	
		                IF EXISTS(SELECT * FROM YUsers WHERE Id = @id AND IsBlocked = 1 AND OwnerId != @ownerId) BEGIN
			                ROLLBACK TRANSACTION
			                RETURN -1
		                END

		                UPDATE YUsers 
		                SET 
			                FirstName = @firstName, 
			                LastName = @lastName,
			                OwnerId = NULL,
			                IsBlocked = 0
		                WHERE Id = @id

		                COMMIT TRANSACTION
	                END TRY
	                BEGIN CATCH
		                IF XACT_STATE() <> 0 
		                ROLLBACK TRANSACTION
	                RAISERROR ('Error in executing transaction', 16, 1)
	                END CATCH
                RETURN 0
                */
                #endregion
                using (var command = new SqlCommand(Settings.Default.UpdateYUserSPName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("firstName", firstName);
                    command.Parameters.AddWithValue("lastName", lastName);
                    command.Parameters.AddWithValue("ownerId", ownerId);

                    var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();

                    command.ExecuteNonQuery();

                    result = (int)returnParameter.Value;
                }
            }

            return result;
        }

        public int ReleaseYUser(int id, string ownerId)
        {
            var result = -1;
            using (var connection = new SqlConnection(ConnectionString))
            {
                #region SQL
                /*
                CREATE PROCEDURE [dbo].[ReleaseYUser]
	                @id int,
	                @ownerId nvarchar(50)
                AS
	                IF EXISTS(SELECT * FROM YUsers WHERE Id = @id AND IsBlocked = 0 AND OwnerId != @ownerId) RETURN -1

	                UPDATE YUsers 
	                SET 
		                OwnerId = NULL,
		                IsBlocked = 0
	                WHERE Id = @id

                RETURN 0
                */
                #endregion
                using (var command = new SqlCommand(Settings.Default.ReleaseYUserSPName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("ownerId", ownerId);

                    var returnParameter = command.Parameters.Add("@ReturnVal", SqlDbType.Int);
                    returnParameter.Direction = ParameterDirection.ReturnValue;

                    connection.Open();

                    command.ExecuteNonQuery();

                    result = (int)returnParameter.Value;
                }
            }

            return result;
        }
    }
}