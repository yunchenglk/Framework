using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YunChengLK.Framework.Data;

namespace YunChengLK.Framework.Dome
{
    [Table("USER")]
    [Serializable]
    public partial class USER : IEntity
    {

        private Guid _iD;

        [Column("ID", DbType.Guid, true, false)]
        public Guid ID
        {
            get { return _iD; }
            set { _iD = value; }
        }

        private String _loginName;

        [Column("LoginName", DbType.String, false, false)]
        public String LoginName
        {
            get { return _loginName == null ? string.Empty : _loginName; }
            set { _loginName = value; }
        }

        private String _loginPassword;

        [Column("LoginPassword", DbType.String, false, false)]
        public String LoginPassword
        {
            get { return _loginPassword == null ? string.Empty : _loginPassword; }
            set { _loginPassword = value; }
        }

        private Int32 _type;

        [Column("Type", DbType.Int32, false, false)]
        public Int32 Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private DateTime _regTime;

        [Column("RegTime", DbType.DateTime, false, false)]
        public DateTime RegTime
        {
            get { return _regTime; }
            set { _regTime = value; }
        }

        private DateTime _createTime;

        [Column("CreateTime", DbType.DateTime, false, false)]
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }

        private Boolean _dR;

        [Column("DR", DbType.Boolean, false, false)]
        public Boolean DR
        {
            get { return _dR; }
            set { _dR = value; }
        }
    }
}
