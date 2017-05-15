using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices.Model;
using scaling_microservices.Rabbit;
using Newtonsoft.Json;

namespace scaling_microservices.Proxy
{
    public class GroupProxy : BasicProxy
    {
        public GroupProxy(string route = "", string exchange = "") : base(route, exchange) { }

        public List<GroupModel_simplified> GetGroups()
        {
            var req = new QueueRequest() { method = "get_groups" };
            Send(req);
            var response = endpoint.Recieve();
            return JsonConvert.DeserializeObject<List<GroupModel_simplified>>(response.StringBody);
        }
        public GroupModel GetGroup(int groupId = 0, string groupName = "", string creatorName = "", int creatorId = 0)
        {
            var req = new QueueRequest() { method = "get_group" };
            req["group_id"] = groupId.ToString();
            req["group_name"] = groupName;
            req["creator_name"] = creatorName;
            req["creator_id"] = creatorId.ToString();

            Send(req);
            return JsonConvert.DeserializeObject<GroupModel>(endpoint.Recieve().StringBody);
        }

        public GroupModel_simplified AddGroup(string groupName, string ownerId, int creatorId)
        {
            var req = new QueueRequest() { method = "create_group" };
            req["group_name"] = groupName;
            req["owner_id"] = ownerId;
            req["creator_id"] = creatorId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<GroupModel_simplified>(endpoint.Recieve().StringBody);
        }

        public bool DeleteGroup(int groupId, int creatorId)
        {
            var req = new QueueRequest() { method = "delete_group" };
            req["group_id"] = groupId.ToString();
            req["creator_id"] = creatorId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public bool EditGroup(int groupId, int creatorId, int newCreatorId = 0, string newName = "")
        {
            var req = new QueueRequest() { method = "edit_group" };
            req["group_id"] = groupId.ToString();
            req["creator_id"] = creatorId.ToString();
            req["new_creator_id"] = newCreatorId.ToString();
            req["new_name"] = newName;
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public bool AddUserToGroup(int groupId, int creatorId, int userId, int roleId)
        {
            var req = new QueueRequest() { method = "add_user" };
            req["group_id"] = groupId.ToString();
            req["creator_id"] = creatorId.ToString();
            req["user_id"] = userId.ToString();
            req["role_id"] = roleId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public bool RemoveUserFromGroup(int groupId, int creatorId, int userId)
        {
            var req = new QueueRequest() { method = "remove_user" };
            req["group_id"] = groupId.ToString();
            req["creator_id"] = creatorId.ToString();
            req["user_id"] = userId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public RoleModel_simplified CreateRole(int creatorId, string name, string owner)
        {
            var req = new QueueRequest() { method = "create_role" };
            req["creator_id"] = creatorId.ToString();
            req["name"] = name;
            req["owner"] = owner;
            Send(req);
            return JsonConvert.DeserializeObject<RoleModel_simplified>(endpoint.Recieve().StringBody);
        }

        public bool AddToRole(int roleId, int addedId, int creatorId)
        {
            var req = new QueueRequest() { method = "add_to_role" };
            req["role_id"] = roleId.ToString();
            req["creator_id"] = creatorId.ToString();
            req["user_to_add_id"] = addedId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        // callerId should either == userId || creatorId
        public bool RemoveFromRole(int roleId, int userId, int callerId = 0)
        {
            var req = new QueueRequest() { method = "remove_from_role" };
            req["role_id"] = roleId.ToString();
            req["caller_id"] = callerId.ToString();
            req["user_id"] = userId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public bool DeleteRole(int roleId, int creatorId)
        {
            var req = new QueueRequest() { method = "delete_role" };
            req["role_id"] = roleId.ToString();
            req["creator_id"] = creatorId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<bool>(endpoint.Recieve().StringBody);
        }

        public RoleModel_simplified EditRole(int roleId, int creatorId, int newOwnerId = 0, string newName = "")
        {
            var req = new QueueRequest() { method = "edit_role" };
            req["role_id"] = roleId.ToString();
            req["creator_id"] = creatorId.ToString();
            req["new_owner_id"] = newOwnerId.ToString();
            req["new_name"] = newName;
            Send(req);
            return JsonConvert.DeserializeObject<RoleModel_simplified>(endpoint.Recieve().StringBody);
        }

        public RoleModel_simplified GetRole(int roleId)
        {
            var req = new QueueRequest() { method = "get_role" };
            req["role_id"] = roleId.ToString();
            Send(req);
            return JsonConvert.DeserializeObject<RoleModel_simplified>(endpoint.Recieve().StringBody);
        }
        //getrole getallroles
    }
}
