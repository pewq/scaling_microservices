using System.Collections.Generic;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;
using scaling_microservices.Entity;
using System.Linq;
using scaling_microservices.Model;
using scaling_microservices.Proxy;

namespace group_service
{
    class GroupService : IService
    {

        public int Port { get; private set; }
        public GroupService(string queueName) : base(queueName)
        {
            ThisInit();
        }
        private GroupService(string queueName, string port)
            : this(queueName, int.Parse(port))
        {
            ThisInit();
        }

        private GroupService(string queueName, int port) :
            base(queueName, port.ToString())
        {
            this.Port = port;
            ThisInit();
        }

        private void ThisInit()
        {
            this.Handlers.Add("get_groups", (RequestHandleDelegate)GetGroupsHandler);
            this.Handlers.Add("create_group", (RequestHandleDelegate)CreateGroupHandler);
            this.Handlers.Add("delete_group", (RequestHandleDelegate)DeleteGroupHandler);
            this.Handlers.Add("edit_group", (RequestHandleDelegate)EditGroupHandler);
            this.Handlers.Add("add_user", (RequestHandleDelegate)AddUserToGroupHandler);
            this.Handlers.Add("remove_user", (RequestHandleDelegate)RemoveUserFromGroupHandler);
            this.Handlers.Add("create_role", (RequestHandleDelegate)CreateRoleHandler);
            this.Handlers.Add("add_to_role", (RequestHandleDelegate)AddUserToRoleHandler);
            this.Handlers.Add("remove_from_role", (RequestHandleDelegate)RemoveUserFromRoleHandler);
            this.Handlers.Add("delete_role", (RequestHandleDelegate)DeleteRoleHandler);
            this.Handlers.Add("edit_role", (RequestHandleDelegate)EditRoleHandler);
            this.Handlers.Add("get_role", (RequestHandleDelegate)GetRoleHandler);
        }

        private void GetGroupsHandler(QueueRequest req)
        {
            using (var ctx = new GroupContext())
            {
                var listValue = JsonConvert.DeserializeObject<List<string>>(req.Contains("group_ids"));
                HashSet<GroupModel_simplified> items = new HashSet<GroupModel_simplified>();
                foreach (var item in ctx.Groups.Where(x => listValue.Contains(x.GroupId.ToString())))
                {
                    items.Add(item);
                }
                listValue = JsonConvert.DeserializeObject<List<string>>(req.Contains("group_names"));
                foreach(var item in ctx.Groups.Where(x => listValue.Contains(x.Name)))
                {
                    items.Add(item);
                }
                if (items.Any())
                {
                    OnResponse(req.properties, items);
                    return;
                }
                var value = req.Contains("group_id");
                if(!string.IsNullOrEmpty(value))
                {
                    var idValue = int.Parse(value);
                    var model = ctx.Groups.SingleOrDefault(x => x.GroupId == idValue);
                    OnResponse(req.properties, model);
                    return;
                }
                value = req.Contains("group_name");
                if (!string.IsNullOrEmpty(value))
                {
                    var models = ctx.Groups.Where(x => x.Name == value);
                    OnResponse(req.properties, models);
                    return;
                }

                value = req.Contains("owner_id");
                if(!string.IsNullOrEmpty(value))
                {
                    var models = ctx.Groups.Where(x => value == x.CreatorId.ToString());
                    OnResponse(req.properties, models);
                    return;
                }
                OnException(new System.ArgumentException("no valid arguments found in request"), req);
            }
        }

        private void CreateGroupHandler(QueueRequest req)
        {
            GroupModel_simplified model;
            try
            {
                model = new GroupModel_simplified()
                {
                    Name = req["group_name"],
                    Owner = req["owner"],
                    CreatorId = int.Parse(req["creator_id"]),
                    Participants = new Dictionary<int, HashSet<int>>()
                };
            }
            catch
            {
                OnException(new System.ArgumentException("invalid arguments for creating UserModel"), req);
                return;
            }
            var clientProxy = new ClientProxy(req["client_routing"], req["client_exchange"]);
            if(model.CreatorId <= 0)
            {
                OnException(new System.ArgumentException("invalid user id"), req);
                return;
            }
            using(var ctx = new GroupContext())
            {
                ctx.Groups.Add(model);
                ctx.SaveChanges();
                OnResponse(req.properties, model);
            }
        }

        private void DeleteGroupHandler(QueueRequest req)
        {
            int creatorId = -1, groupId = -1;
            try
            {
                creatorId = int.Parse(req["creator_id"]);
                groupId = int.Parse(req["group_id"]);
            }
            catch
            {
                OnException(new System.ArgumentException("request has invalid argumens"), req);
            }
            using (var ctx = new GroupContext())
            {
                var entity = ctx.Groups.SingleOrDefault(x => x.GroupId == groupId && x.CreatorId == creatorId);
                if(entity == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                ctx.Groups.Remove(entity);
                OnResponse(req.properties, true);
            }
        }

        private void EditGroupHandler(QueueRequest req)
        {
            int groupId = -1, creatorId = -1, newCreatorId = 0;
            try
            {
                groupId = int.Parse(req["groupd_id"]);
                creatorId = int.Parse(req["creator_id"]);
                int.TryParse(req["new_creator_id"], out newCreatorId);
            }
            catch
            {
                OnException(new System.ArgumentException("request has invalid arguments"), req);
                return;
            }
            using (var ctx = new GroupContext())
            {
                var entity = ctx.Groups.SingleOrDefault(x => x.GroupId == groupId);
                if(entity == null)
                {
                    OnException(new System.ArgumentException("request has invalid group id"), req);
                    return;
                }
                string newName = req.Contains("new_name");
                if (!string.IsNullOrEmpty(newName))
                {
                    entity.Name = newName;
                }
                if(newCreatorId != 0)
                {
                    //TODO : decide, whether to pass a user or create proxy and get it
                }
                OnResponse(req.properties, true);
            }
        }
        private void AddUserToGroupHandler(QueueRequest req)
        {
            int roleId = -1, groupId = -1, creatorId = -1, userId = -1;
            try
            {
                roleId = int.Parse(req["role_id"]);
                groupId = int.Parse(req["group_id"]);
                creatorId = int.Parse(req["creator_id"]);
                userId = int.Parse(req["user_id"]);
            }
            catch
            {
                OnException(new System.ArgumentException("invalid arguments"), req);
                return;
            }
            using(var ctx = new GroupContext())
            {
                var group = ctx.Groups.First(x => x.CreatorId == creatorId && x.GroupId == groupId);
                if(group == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                //TODO : get user? or change this shit;
                bool containsUser = ctx.Groups.First(x => x.GroupId == groupId).Participants.ContainsKey(userId);
                if(!containsUser)
                {
                    ctx.Groups.First(x => x.GroupId == groupId).Participants.Add(userId, new HashSet<int>() { roleId });
                }
                else
                {
                    ctx.Groups.First(x => x.GroupId == groupId).Participants[userId].Add(roleId);
                }
                OnResponse(req.properties, true);
                return;
            }
        }
        private void RemoveUserFromGroupHandler(QueueRequest req)
        {
            int groupId = -1, creatorId = -1, userId = -1;
            try
            {
                groupId = int.Parse(req["group_id"]);
                creatorId = int.Parse(req["creator_id"]);
                userId = int.Parse(req["user_id"]);
            }
            catch
            {

            }
            using (var ctx = new GroupContext())
            {
                var group = ctx.Groups.First(x => x.GroupId == groupId && x.CreatorId == creatorId);
                if(group == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                var result = group.Participants.Remove(userId);
                OnResponse(req.properties, true);
            }
        }

        private void CreateRoleHandler(QueueRequest req)
        {
            string owner = "";
            string name = "";
            int creatorId = -1;
            try
            {
                owner = req["owner"];
                name = req["name"];
                creatorId = int.Parse(req["creator_id"]);
            }
            catch
            {

            }
            using (var ctx = new GroupContext())
            {
                var groupModel = new GroupModel_simplified()
                {
                    CreatorId = creatorId,
                    Name = name,
                    Owner = owner,
                    Participants = new Dictionary<int, HashSet<int>>()
                };
                ctx.Groups.Add(groupModel);
                ctx.SaveChanges();
                OnResponse(req.properties, groupModel);
            }
        }
        private void AddUserToRoleHandler(QueueRequest req)
        {
            int roleId = -1, creatorId = -1, userId = -1;
            try
            {
                roleId = int.Parse(req["role_id"]);
                userId = int.Parse(req["user_id"]);
                creatorId = int.Parse(req["creator_id"]);
            }
            catch
            {

            }
            using (var ctx = new GroupContext())
            {
                var role = ctx.Roles.First(x => x.RoleId == roleId && x.Creator == creatorId);
                if(role == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                role.Participants.Add(userId);
                ctx.SaveChanges();
                OnResponse(req.properties, true);
            }
        }
        private void RemoveUserFromRoleHandler(QueueRequest req)
        {
            int callerId = -1, userId = -1, roleId = -1;
            try
            {
                callerId = int.Parse(req["caller_id"]);
                userId = int.Parse(req["user_id"]);
                roleId = int.Parse(req["role_id"]);
            }
            catch
            {

            }
            using (var ctx = new GroupContext())
            {
                RoleModel_simplified role;
                if (callerId == userId)
                {
                    role = ctx.Roles.First(x => x.RoleId == roleId);
                }
                else
                {
                    role = ctx.Roles.First(x => x.RoleId == roleId && x.Creator == callerId);
                }
                if (role == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                role.Participants.Remove(userId);
                ctx.SaveChanges();
                OnResponse(req.properties, true);
            }
        }
        private void DeleteRoleHandler(QueueRequest req)
        {
            int creatorId = -1, roleId = -1;
            try
            {
                creatorId = int.Parse(req["caller_id"]);
                roleId = int.Parse(req["role_id"]);
            }
            catch
            {

            }
            using (var ctx = new GroupContext())
            {
                
                var role = ctx.Roles.First(x => x.RoleId == roleId && x.Creator == creatorId);
                if (role == null)
                {
                    OnResponse(req.properties, false);
                    return;
                }
                ctx.Roles.Remove(role);
                ctx.SaveChanges();
                OnResponse(req.properties, true);
            }
        }
        private void EditRoleHandler(QueueRequest req)
        {
            int roleId = 0, creatorId = 0, newOwner = 0;
            string newName = "";
            try
            {
                creatorId = int.Parse(req["creator_id"]);
                roleId = int.Parse(req["role_id"]);
                newOwner = int.Parse(req["new_owner_id"]);
                newName = req["new_name"];
            }
            catch
            {

            }
            using (var ctx = new GroupContext())
            {
                var role = ctx.Roles.First(x => x.RoleId == roleId && x.Creator == creatorId);
                if(newOwner != 0)
                {
                    role.Creator = newOwner;
                }
                if (newName != "")
                {
                    role.Name = newName;
                }
                ctx.SaveChanges();
                OnResponse(req.properties, role);
            }
        }

        private void GetRoleHandler(QueueRequest req)
        {
            int roleId = 0;
            try
            {
                roleId = int.Parse(req["role_id"]);
            }
            catch { }
            using (var ctx = new GroupContext())
            {
                var role = ctx.Roles.First(x => x.RoleId == roleId);
                OnResponse(req.properties, role);
            }
        }
    }
}
