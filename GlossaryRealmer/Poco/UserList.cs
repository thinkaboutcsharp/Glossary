using AutoMapper.Configuration.Annotations;
//using PropertyChanged;
using Realmer.Util;
using System;
using System.Collections.Generic;

namespace Realmer.Poco
{
    //[AddINotifyPropertyChangedInterface]
    public class UserList : PocoBase<UserList, int>
    {
        public int UserId { get; set; }
        [Ignore]
        public IReadOnlyList<User> Users { get; }

        public UserList() : base(o => o.UserId, () => new Scheme.UserList())
        {
            Users = AddListProperty<User, Scheme.UserList, Scheme.User>(o => o.Users);
        }
    }
}
