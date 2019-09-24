using AutoMapper.Configuration.Annotations;
using Realms;
using System.Collections.Generic;

namespace Realmer.Scheme
{
    internal class UserList : RealmObject
    {
        internal int PK => UserId;

        [PrimaryKey]
        public int UserId { get; set; }
        [Ignore]
        public IList<User> Users { get; }
    }
}
