﻿using System.Data.Entity;
using scaling_microservices.Model;



namespace scaling_microservices.Entity
{
    public class GroupContext : DbContext
    {
        public DbSet<GroupModel> Groups;

        public GroupContext() : base() { }

        public GroupContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
    }
}
