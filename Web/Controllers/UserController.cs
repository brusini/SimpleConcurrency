using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using BusinessLayer.Interfaces;
using SimpleConcurrency.BusinessLayer;
using SimpleConcurrency.Model;
using SimpleConcurrency.Web.Helpers;
using SimpleConcurrency.Web.Models;

namespace SimpleConcurrency.Web.Controllers
{
    [RoutePrefix("api")]
    public class UserController : ApiController
    {
        private readonly IBusinessService _bllService;

        public UserController(IBusinessService service)
        {
            _bllService = service;
        }

        public UserController()
        {
            _bllService = new UserService();
        }

        [Route("users/y"), HttpGet]
        [ResponseType(typeof(List<YUser>))]
        public IHttpActionResult GetYUsers()
        {
            var users = new UserService().GetUsersY();

            return Ok(users);
        }

        [Route("users/x"), HttpGet]
        [ResponseType(typeof(List<XUser>))]
        public IHttpActionResult GetXUsers()
        {
            var users = _bllService.GetUsersX();

            users.ForEach(r => { r.EpochUpdateDate = DateTimeHelper.ToUnixTime(r.UpdateDate); });

            return Ok(users);
        }

        [Route("user/x/{id:int}"), HttpGet]
        [ResponseType(typeof(XUser))]
        public IHttpActionResult GetXUser(int id)
        {
            if (id <= 0) ModelState.AddModelError("id", "Required");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _bllService.GetUserX(id);

            user.EpochUpdateDate = DateTimeHelper.ToUnixTime(user.UpdateDate); 

            return Ok(user);
        }

        [Route("user/y/{id:int}/edit/owner/{owner}"), HttpGet]
        [ResponseType(typeof(YUser))]
        public IHttpActionResult GetYUserForEditing(int id, string owner)
        {
            if (id <= 0) ModelState.AddModelError("id", "Required");
            if (string.IsNullOrEmpty(owner)) ModelState.AddModelError("owner", "Required");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = _bllService.GetYUserForEditing(id, owner);

            return Ok(user);
        }

        [Route("user/y/{id:int}/release/owner/{owner}"), HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult ReleaseYUser(int id, string owner)
        {
            if (id <= 0) ModelState.AddModelError("id", "Required");
            if (string.IsNullOrEmpty(owner)) ModelState.AddModelError("owner", "Required");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var res = _bllService.ReleaseYUser(id, owner);

            return Ok(res);
        }

        [Route("user/x"), HttpPut]
        [ResponseType(typeof(BaseUserModel))]
        public IHttpActionResult AddXUser(BaseUserModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var u = new BaseUserModel();
            var createDate = DateTime.Now;
            u.Id = _bllService.AddYUser(model.FirstName, model.LastName, false, createDate);
            u.EpochUpdateDate = DateTimeHelper.ToUnixTime(createDate);
            return Ok(u);
        }

        [Route("user/y"), HttpPut]
        [ResponseType(typeof(int))]
        public IHttpActionResult AddYUser(BaseUserModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var res = _bllService.AddXUser(model.FirstName, model.LastName, false, null);

            return Ok(res);
        }

        [Route("user/x"), HttpPost]
        [ResponseType(typeof(BaseUserModel))]
        public IHttpActionResult UpdateXUser(BaseUserModel model)
        {
            if (model.Id <= 0) ModelState.AddModelError("Id", "Required");
            if (model.EpochUpdateDate <=0 ) ModelState.AddModelError("EpochUpdateDate", "Required");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newDate = DateTime.Now;

            var res = _bllService.UpdateXUser(model.Id, model.FirstName, model.LastName, DateTimeHelper.FromUnixTime(model.EpochUpdateDate), newDate);

            if (res == -1) model.Id = -1; // Deleted user
            else if (res == -2)
            {
                // Version changed. Do nothing. Leave data as is. Client app detect 'no change' and ask user to fix.
            }
            else model.EpochUpdateDate = DateTimeHelper.ToUnixTime(newDate); // record updated

            return Ok(model);
        }

        [Route("user/y"), HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult UpdateYUser(BaseUserModel model)
        {
            if (model.Id <= 0) ModelState.AddModelError("Id", "Required");
            if (string.IsNullOrEmpty(model.OwnerId)) ModelState.AddModelError("OwnerId", "Required");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var res = _bllService.UpdateYUser(model.Id, model.FirstName, model.LastName, model.OwnerId);

            return Ok(res);
        }

        [Route("user/x"), HttpDelete]
        [ResponseType(typeof(int))]
        public IHttpActionResult DeleteXUser(DeleteXUserModel model)
        {
            if (model.EpochUpdateDate <= 0) ModelState.AddModelError("EpochUpdateDate", "Required");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var res = _bllService.DeleteXUser(model.Id, DateTimeHelper.FromUnixTime(model.EpochUpdateDate));

            return Ok(res);
        }

        [Route("user/y"), HttpDelete]
        [ResponseType(typeof(int))]
        public IHttpActionResult DeleteYUser(DeleteYUserModel model)
        {
            if (string.IsNullOrEmpty(model.OwnerId)) ModelState.AddModelError("OwnerId", "Required");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var res = _bllService.DeleteYUser(model.Id, model.OwnerId);

            return Ok(res);
        }
    }
}