using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;
using scaling_microservices.Proxy.Model;
using scaling_microservices.StorageStub;

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
            this.Handlers.Add("get_group", (RequestHandleDelegate)SelectGroupHandler);
            this.Handlers.Add("create_group", (RequestHandleDelegate)CreateGroupHandler);
            this.Handlers.Add("delete_group", (RequestHandleDelegate)DeleteGroupHandler);
            this.Handlers.Add("edit_group", (RequestHandleDelegate)EditGroupHandler);
            this.Handlers.Add("add_user", (RequestHandleDelegate)AddUserToGroupHandler);
            this.Handlers.Add("remove_user", (RequestHandleDelegate)RemoveUserFromGroupHandler);
            this.Handlers.Add("create_role", (RequestHandleDelegate)CreateToleHandler);
            this.Handlers.Add("add_to_role", (RequestHandleDelegate)AddUserToRole);
            this.Handlers.Add("remove_from_role", (RequestHandleDelegate)RemoveUserFromRoleHandler);
            this.Handlers.Add("delete_role", (RequestHandleDelegate)DeleteRoleHandler);
            this.Handlers.Add("edit_role", (RequestHandleDelegate)EditRoleHandler);
        }

        private void GetGroupsHandler(QueueRequest req)
        {

        }

        private void SelectGroupHandler(QueueRequest req)
        {

        }

        private void CreateGroupHandler(QueueRequest req)
        {

        }

        private void DeleteGroupHandler(QueueRequest req)
        {

        }

        private void EditGroupHandler(QueueRequest req)
        {

        }

        private void AddUserToGroupHandler(QueueRequest req)
        {

        }
        private void RemoveUserFromGroupHandler(QueueRequest req)
        {

        }

        private void CreateToleHandler(QueueRequest req)
        {

        }
        private void AddUserToRole(QueueRequest req)
        {

        }
        private void RemoveUserFromRoleHandler(QueueRequest req)
        {

        }
        private void DeleteRoleHandler(QueueRequest req)
        {

        }
        private void EditRoleHandler(QueueRequest req)
        {

        }
    }
}
