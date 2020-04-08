using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication4.Models;
namespace WebApplication4.Controllers
{
    public class JQueryAjaxController : Controller
    {
        private MyDbEntities1 db = new MyDbEntities1();

        [HttpPost]
        public ActionResult PostResult(result r)
        {
            r.report_time = DateTime.Now;
            db.result.Add(r);
            db.SaveChanges();
            return  Json(new
            {
                success = "yes"
            });
        }
        
        public JsonResult GetIndividualResult()
        {
            String mp = Request["mp"];
            String dp = Request["dp"];


            DateTime b=new DateTime(), e = new DateTime();
            if (dp != "")
            {
                b= Convert.ToDateTime(dp);
                e = b.AddDays(1);
            }
            if (mp != "")
            {
                b = Convert.ToDateTime(mp + "/1");
                e = b.AddMonths(1);
            }
            var query = from r in db.result
                        where (r.report_time >= b && r.report_time < e)
                        group r by r.member_id into g
                        select new
                        {
                            No = g.Key,
                            Dials = g.Select(x => x.dial).Sum(),
                            Contacts = g.Select(x => x.contact).Sum(),
                            Qt = g.Select(x => x.qt).Sum(),

                        } into my_view
                        join m in db.member
                        on my_view.No equals m.id
                        join t in db.team
                        on m.team_id equals t.id
                        select new
                        {
                            No = my_view.No,
                            Dials = my_view.Dials,
                            Contacts = my_view.Contacts,
                            Qt = my_view.Qt,
                            Name = m.member_name,
                            Team = t.team_name
                        };
            return Json(query,JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTeamResult()
        {
            String mp = Request["mp"];
            String dp = Request["dp"];


 
            DateTime b = new DateTime(), e = new DateTime();
            if (dp != "")
            {
                b = Convert.ToDateTime(dp);
                e = b.AddDays(1);
            }
            if (mp != "")
            {
                b = Convert.ToDateTime(mp + "/1");
                e = b.AddMonths(1);
            }
            var query = from r in db.result
                        join m in db.member
                        on r.member_id equals m.id
                        join t in db.team
                        on m.team_id equals t.id
                        where (r.report_time >= b && r.report_time < e)
                        select new
                        {
                            Dials = r.dial,
                            Contacts = r.contact,
                            Qt = r.qt,
                            Name = t.team_name

                        } into my_view
                        group my_view by my_view.Name into g
                        select new
                        {
                            Dials = g.Select(x => x.Dials).Sum(),
                            Contacts = g.Select(x => x.Contacts).Sum(),
                            Qt = g.Select(x => x.Qt).Sum(),
                            Team = g.Key
                        };
                       
            return Json(query, JsonRequestBehavior.AllowGet);

        }


    }
}