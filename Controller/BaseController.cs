using System;
using System.Data;
using System.Windows.Forms;

namespace CuahangNongduoc.Controller
{
    public abstract class BaseController
    {
        protected BindingSource bs = new BindingSource();

        protected BaseController()
        {
            bs.DataSource = LayDuLieuRong();
        }

        protected abstract DataTable LayDuLieuRong();
    }
}
