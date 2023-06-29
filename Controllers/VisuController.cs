/*ALL IMPORTS*/
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.AspNetCore.Http.HttpRequest;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using Adient_DashBoard.Models;
using System.Threading;
/*END OF IMPORTS*/



namespace Adient_DashBoard.Controllers
{
    public class VisuController : Controller
    {
        /*ALL GLOBAL VARIABLES*/


        public db_globalContext _ctx = new db_globalContext();
        //Now I can call the global database with _ctx and not doing at each time : "db_globalContext", that's easier

        public db_dashboardContext _ctx2 = new db_dashboardContext();
        //The same for _ctx2 ...

        public static TConfig ProductionSelected = new TConfig();
        //I keep in memory the Production Hall selected so I can't lose it when I go from action to action.

        public static TConfig ShiftSelected = new TConfig();
        //Same as ProductionSelected

        public static TConfig SectionSelected = new TConfig();
        //Same as ProductionSelected

        public static DateTime DateSelected = new DateTime();
        //Same as ProductionSelected, more precisely when I want to display the reports and edit them, I don't have to search among all reports.
        //I do it once and I keep it for all operations.

        public static DateTime dataPlan_dateSelected = new DateTime();
        //When I want to Edit a dataplan on the server, I need to keep it in memory so I had to add this line. 

        public static List<int> listShiftIdViewData = new List<int>();
        //I use this list to gather all shift IDs to see when I want to consult a report (because there are 3 shift ID, one for 4.0, one for 4.1, and one again for AMG)
        
        public static string langSelected="EN";
        //English is the language by default

        public static TUserAccess CurrentUser = new TUserAccess();
        //Current User is stored in this variable, if I'm not logged, CurrentUser is null.So I don't have access to many services.


        public static int AfficheBand = 0;


        static int currentmonthnum;


        /*END OF GLOBAL VARIABLES*/





        //HOME PAGE *****************************************************************************************************
        public IActionResult Index()
        {
            /*This is the default action, so when I launch the program or when I type a-690mv76.autoexpr.com/dashboard, I have the view of this action*/

            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            ViewBag.langSelected = langSelected;

            /*Those one are for the navigation bar. So I have to copy those 5 lines in all actions which redirect to Views because in each view,
             we can access to the navigation bar.*/
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            


            /*Those one are for the body of the page*/
            ViewBag.homepage = _ctx2.TTranslations.Where(s => (s.FLabelid == 48) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login_mess = _ctx2.TTranslations.Where(s => (s.FLabelid == 50) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.headcount_prod = _ctx2.TTranslations.Where(s => (s.FLabelid == 55) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.statusITsys = _ctx2.TTranslations.Where(s => (s.FLabelid == 89) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.efficiency = _ctx2.TTranslations.Where(s => (s.FLabelid == 90) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.see = _ctx2.TTranslations.Where(s => (s.FLabelid == 64) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            /*There is nothing to do, I just take the different messages and words that I need to display following langSelected*/



            return View();
        }


        //LOG IN ***************************************************************************************************
        public IActionResult Connexion()
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            ViewBag.signinto = _ctx2.TTranslations.Where(s => (s.FLabelid == 76) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.signin = _ctx2.TTranslations.Where(s => (s.FLabelid == 77) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.userid = _ctx2.TTranslations.Where(s => (s.FLabelid == 78) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.mdp = _ctx2.TTranslations.Where(s => (s.FLabelid == 79) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
           
            ViewBag.langSelected = langSelected;


            /*This action don't really make the operation of logging in, it just gather messages and labels to display. It displays the
             log in page.*/

            return View();
        }

        [HttpPost]
        public IActionResult ResultConnexion(string userid, string password)
        {
            /*Here is the operation of logging in. I use the Post method so I have 2 arguments: userId and the password typed by the user. If
             you want to see how can we send those informations to an action, you can look the view named Connexion. I just used the 
            flag:     <form asp-action="ResultConnexion">        at the beginning of the form. 
            
            So I tell him to send

            <input type="password" class="form-control form-control mb-3" name="password" id="password" placeholder="Password" required>

            and 

             <input type="text" class="form-control form-control mb-3" id="userid" name="userid" placeholder="User ID" required>

            to the action named ResultConnexion. Notice that the parameters of this action have the same name as the input of the view*/

            TUser temp = new TUser();
            temp = (TUser)_ctx.TUsers.Where(s => s.FUserId.Equals(userid)).FirstOrDefault(); /*I search if the userId typed exists*/
            if (temp != null) /*If the userid exist, I now want to see if the password matches*/
            {

                if (MD5Hash.Hash.Content(password).Equals(temp.FPassword))/*I see if the hashed password typed by the user equals to the passwords hashed in the database*/
                {
                    CurrentUser.FUserId = temp.FUserId;
                    TUserAccess uRole = _ctx.TUserAccesses.Where(s => s.FApplication.Equals("Dashboard")).Where(s => s.FUserId.Equals(CurrentUser.FUserId)).FirstOrDefault();
                    CurrentUser.FRole = uRole.FRole;
                    /*If the password matches, CurrentUser is now not null and I redirect to Index with the view of a logged in user*/
                    return RedirectToAction("Index");
                }
                else
                {
                    /*If the passwords don't match, I return an error message thank to the action ErreurConnexionBadPassword */
                    return RedirectToAction("ErreurConnexionBadPassword");
                }
            }
            else
            {
                /*if the userid doesn't exist, I return an error message thanks to the action ErreurConnexionEmailIntrouvable */
                return RedirectToAction("ErreurConnexionEmailIntrouvable");
            }
        }
        public IActionResult LogOut()
        {
            /*We arrive here only by clicking on the button to log out. You can see how I do it in the View named _LoginPartial with :  
             * 
              <a class="nav-link" asp-area="" asp-controller="Visu" asp-action="LogOut"> @ViewBag.logout.FTranslation </a>
            
             So it's a navlink in which I specified the Controller and the action. Then I turn CurrentUser to null.
             */
            CurrentUser.FUserId = null;
            CurrentUser.FRole = null;
            return RedirectToAction("Index");
        }

        public IActionResult ErreurConnexionBadPassword()
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            ViewBag.errordetect = _ctx2.TTranslations.Where(s => (s.FLabelid == 80) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.errormess = _ctx2.TTranslations.Where(s => (s.FLabelid == 83) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.backtologin = _ctx2.TTranslations.Where(s => (s.FLabelid == 82) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            ViewBag.langSelected = langSelected;

            /*I gather all I need to display the error message.*/

            return View();
        }

        public IActionResult ErreurConnexionEmailIntrouvable()
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            ViewBag.errordetect = _ctx2.TTranslations.Where(s => (s.FLabelid == 80) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.errormail = _ctx2.TTranslations.Where(s => (s.FLabelid == 81) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.backtologin = _ctx2.TTranslations.Where(s => (s.FLabelid == 82) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;

            /*I gather all I need to display the error message.*/
            return View();
        }

        public IActionResult ListeTagesreport(int? page, TConfig config)
        {
            /* This action is done to choose the year of the report we want to see. After choosing the year, we choose the month (ListeTagesreportMonth), then the day(ListeTagesreportDays).*/



            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            AfficheBand = 0;
            ProductionSelected = null;
            ShiftSelected = null;
            SectionSelected = null;

            List<int> listYear = new List<int>();
            List<int> listYearWithoutDuplicates = new List<int>();

            
           



            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            /*Those loops are used to only give access to logged users, so if you're not logged, you have to log in. And it's also a filter.
             This action is reachable by users who has an editor role (in db_global mysql database). It can be changed as you want.*/

            else
            {
                //ViewBag.Configs = new SelectList(_ctx2.TConfigs.ToList().Where(s => s.FConfigid <= 4), "FConfigid", "FField");

                string sql = "EXEC dbo.Get_config";//sql is a query for the database, it launches the stocked procedure Get_config.


                List<TShift> listeShifts = _ctx2.TShifts.FromSqlRaw<TShift>(sql).ToList();
                /*To understand this line: on _ctx2, so on the SQL Database, I execute the stocked procedure on t_shifts table*/



                foreach (var item in listeShifts)
                {
                    listYear.Add(DateAndTime.Year(item.FDate));//List of all years of reports


                }
                listYearWithoutDuplicates = listYear.Distinct().ToList();//In the case if there are 2 reports of 2022, I only want to know that there are report in 2022 once, not twice or more.
                listYearWithoutDuplicates.Sort();
                ViewBag.Years = listYearWithoutDuplicates;


                ViewBag.select = _ctx2.TTranslations.Where(s => (s.FLabelid == 61) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.headcount_prod = _ctx2.TTranslations.Where(s => (s.FLabelid == 55) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.addreport = _ctx2.TTranslations.Where(s => (s.FLabelid == 56) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.reportdate = _ctx2.TTranslations.Where(s => (s.FLabelid == 57) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.addbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 58) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.cancelbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 59) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.action = _ctx2.TTranslations.Where(s => (s.FLabelid == 60) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();





                return View();
            }

        }


        public IActionResult ListeTagesreportMonth(TConfig config, int YearSelected)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            ViewBag.CurrentYear = YearSelected;

            List<string> listMonth = new List<string>();
            List<string> listMonthWithoutDuplicates = new List<string>();



            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                ViewBag.Configs = new SelectList(_ctx2.TConfigs.ToList().Where(s => s.FConfigid <= 4), "FConfigid", "FField");
                string sql = "EXEC dbo.Get_config";
                List<TShift> listeShifts = _ctx2.TShifts.FromSqlRaw<TShift>(sql).ToList();
                List<TShift> listeShiftsConcerned = listeShifts.Where(s => DateAndTime.Year(s.FDate) == YearSelected).ToList();
                foreach (var item in listeShiftsConcerned)
                {
                    listMonth.Add(DateAndTime.MonthName(DateAndTime.Month(item.FDate)));
                }
                listMonthWithoutDuplicates = listMonth.Distinct().ToList();
                ViewBag.Month = listMonthWithoutDuplicates;


                ViewBag.select = _ctx2.TTranslations.Where(s => (s.FLabelid == 61) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.headcount_prod = _ctx2.TTranslations.Where(s => (s.FLabelid == 55) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.addreport = _ctx2.TTranslations.Where(s => (s.FLabelid == 56) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.reportdate = _ctx2.TTranslations.Where(s => (s.FLabelid == 57) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.addbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 58) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.cancelbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 59) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.action = _ctx2.TTranslations.Where(s => (s.FLabelid == 60) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.reportmonth = _ctx2.TTranslations.Where(s => (s.FLabelid == 62) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


                return View();
            }

        }

        public IActionResult ListeTagesreportDays(TConfig config, int YearSelected, string MonthSelected)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            ViewBag.CurrentYear = YearSelected;
            ViewBag.CurrentMonth = MonthSelected;



            List<int> listDays = new List<int>();
            List<int> listDayWithoutDuplicates = new List<int>();



            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                ViewBag.Configs = new SelectList(_ctx2.TConfigs.ToList().Where(s => s.FConfigid <= 4), "FConfigid", "FField");
                string sql = "EXEC dbo.Get_config";
                List<TShift> listeShifts = _ctx2.TShifts.FromSqlRaw<TShift>(sql).ToList();
                List<TShift> listeShiftsConcerned = listeShifts.Where(s => (DateAndTime.Year(s.FDate) == YearSelected) && (DateAndTime.MonthName(DateAndTime.Month(s.FDate)).Equals(MonthSelected))).ToList();
                foreach (var item in listeShiftsConcerned)
                {


                    currentmonthnum = DateAndTime.Month(item.FDate);

                    listDays.Add(DateAndTime.Day(item.FDate));


                }
                listDayWithoutDuplicates = listDays.Distinct().ToList();
                listDayWithoutDuplicates.Sort();
                ViewBag.Days = listDayWithoutDuplicates;
                ViewBag.CurrentMonthNum = currentmonthnum;


                ViewBag.view = _ctx2.TTranslations.Where(s => (s.FLabelid == 64) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.delete = _ctx2.TTranslations.Where(s => (s.FLabelid == 65) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.headcount_prod = _ctx2.TTranslations.Where(s => (s.FLabelid == 55) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.addreport = _ctx2.TTranslations.Where(s => (s.FLabelid == 56) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.reportdate = _ctx2.TTranslations.Where(s => (s.FLabelid == 57) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.addbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 58) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.cancelbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 59) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.action = _ctx2.TTranslations.Where(s => (s.FLabelid == 60) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();



                return View();
            }

        }

        public IActionResult AccessDenied()
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;

            ViewBag.accesdenied = _ctx2.TTranslations.Where(s => (s.FLabelid == 84) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.accessmess = _ctx2.TTranslations.Where(s => (s.FLabelid == 85) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.backtohome = _ctx2.TTranslations.Where(s => (s.FLabelid == 86) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();



            return View();
        }


        //UPDATE ON THE DATABASE
        [HttpPost]
        public IActionResult AddConfig(DateTime ReportDate)
        {
            DateSelected = ReportDate;

            TShift ExistOrNot = _ctx2.TShifts.Where(s => DateTime.Compare(s.FDate, DateSelected) == 0).FirstOrDefault();
            if (ExistOrNot == null)
            {
                string sql = "EXEC dbo.Add_config @ConfigId,@ConfigName,@CurrentUser,@ParamDate";
                List<SqlParameter> parms = new List<SqlParameter>
                    {new SqlParameter { ParameterName = "@ConfigId", Value =1 },
                    new SqlParameter { ParameterName = "@ConfigName", Value ="Hall 4.0" },
                    new SqlParameter { ParameterName = "@CurrentUser", Value = CurrentUser.FUserId },
                    new SqlParameter { ParameterName = "@ParamDate", Value =ReportDate }};
                int rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());

                string sql2 = "EXEC dbo.Add_config @ConfigId,@ConfigName,@CurrentUser,@ParamDate";
                List<SqlParameter> parms2 = new List<SqlParameter>
                    {new SqlParameter { ParameterName = "@ConfigId", Value =2 },
                    new SqlParameter { ParameterName = "@ConfigName", Value ="Hall 4.1" },
                    new SqlParameter { ParameterName = "@CurrentUser", Value = CurrentUser.FUserId },
                    new SqlParameter { ParameterName = "@ParamDate", Value =ReportDate }};
                int rowsAffected2 = _ctx2.Database.ExecuteSqlRaw(sql2, parms2.ToArray());

                string sql3 = "EXEC dbo.Add_config @ConfigId,@ConfigName,@CurrentUser,@ParamDate";
                List<SqlParameter> parms3 = new List<SqlParameter>
                    {new SqlParameter { ParameterName = "@ConfigId", Value =4 },
                    new SqlParameter { ParameterName = "@ConfigName", Value ="Hall AMG" },
                    new SqlParameter { ParameterName = "@CurrentUser", Value = CurrentUser.FUserId },
                    new SqlParameter { ParameterName = "@ParamDate", Value =ReportDate }};
                int rowsAffected3 = _ctx2.Database.ExecuteSqlRaw(sql3, parms3.ToArray());
                return RedirectToAction("ViewData", new { DaySelected = DateSelected.Day, MonthSelected = DateSelected.Month, YearSelected = DateSelected.Year });
            }
            else
            {
                return RedirectToAction("ViewData", new { DaySelected = DateSelected.Day, MonthSelected = DateSelected.Month, YearSelected = DateSelected.Year });
            }




        }

        

        public IActionResult ViewData(int DaySelected, int MonthSelected, int YearSelected)
        {

            int IdAMGShift1;
            int IdAMGShift2;
            int IdAMGShift3;


            int Id40Shift1SectionA;
            int Id40Shift1SectionC;
            int Id40Shift1Prod;
            int Id40Shift2SectionA;
            int Id40Shift2SectionC;
            int Id40Shift2Prod;
            int Id40Shift3SectionA;
            int Id40Shift3SectionC;
            int Id40Shift3Prod;

            int Id41Shift1SectionA;
            int Id41Shift1SectionC;
            int Id41Shift1Prod;
            int Id41Shift2SectionA;
            int Id41Shift2SectionC;
            int Id41Shift2Prod;
            int Id41Shift3SectionA;
            int Id41Shift3SectionC;
            int Id41Shift3Prod;

            listShiftIdViewData.Clear();

            if (langSelected == null)
			{
                langSelected = "EN";
			}

            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;

            ViewBag.DayChosen = DaySelected;
            ViewBag.MonthChosen = MonthSelected;
            ViewBag.YearChosen = YearSelected;

            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();



            ViewBag.DataTypes = _ctx2.TRecordtypes.ToList();
            var date1 = new DateTime();

            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                //TShift OneOfShiftConcerned = _ctx2.TShifts.Where(s => (DateAndTime.Year(s.FDate) == YearSelected) && (DateAndTime.Month(s.FDate) == MonthSelected) && (DateAndTime.Day(s.FDate)==DaySelected)).ToList().FirstOrDefault() ;
                foreach (var item in _ctx2.TShifts)
                {
                    date1 = new DateTime(YearSelected, MonthSelected, DaySelected, 0, 0, 0);
                    int result = DateTime.Compare(item.FDate, date1);
                    if (result == 0)
                    {
                        listShiftIdViewData.Add(item.FShiftid);

                        /*************HALL AMG****************/

                        if ((item.FHall.Equals("Hall AMG")) && (item.FShift.Equals("Layer 1")))
                        {
                            IdAMGShift1 = item.FShiftid;
                            ViewBag.IdAMGShift1 = IdAMGShift1;

                            string sql = "EXEC dbo.Get_Data @ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@ShiftId", Value = IdAMGShift1 }};
                            List<TDatarecord> data_AMG_Shift1 = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                            foreach (var elt in data_AMG_Shift1)
                            {
                                if (elt.FRecordtypeid == 1)
                                {
                                    TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 49)).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        elt.FRecordvalue = temp.FValue;
                                    }
                                }
                            }

                            ViewBag.data_AMG_Shift1 = data_AMG_Shift1;


                        }
                        if ((item.FHall.Equals("Hall AMG")) && (item.FShift.Equals("Layer 2")))
                        {
                            IdAMGShift2 = item.FShiftid;
                            ViewBag.IdAMGShift2 = IdAMGShift2;

                            string sql = "EXEC dbo.Get_Data @ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@ShiftId", Value = IdAMGShift2 }};
                            List<TDatarecord> data_AMG_Shift2 = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                            foreach (var elt in data_AMG_Shift2)
                            {
                                if (elt.FRecordtypeid == 1)
                                {
                                    TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 50)).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        elt.FRecordvalue = temp.FValue;
                                    }
                                }
                            }
                            ViewBag.data_AMG_Shift2 = data_AMG_Shift2;
                        }
                        if ((item.FHall.Equals("Hall AMG")) && (item.FShift.Equals("Layer 3")))
                        {
                            IdAMGShift3 = item.FShiftid;
                            ViewBag.IdAMGShift3 = IdAMGShift3;

                            string sql = "EXEC dbo.Get_Data @ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@ShiftId", Value = IdAMGShift3 }};
                            List<TDatarecord> data_AMG_Shift3 = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                            foreach (var elt in data_AMG_Shift3)
                            {
                                if (elt.FRecordtypeid == 1)
                                {
                                    TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 51)).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        elt.FRecordvalue = temp.FValue;
                                    }
                                }
                            }
                            ViewBag.data_AMG_Shift3 = data_AMG_Shift3;
                        }

                        /*************HALL 4.0****************/

                        if ((item.FHall.Equals("Hall 4.0")) && (item.FShift.Equals("Layer 1")))
                        {
                            if (item.FSection.Equals("A"))
                            {
                                Id40Shift1SectionA = item.FShiftid;
                                ViewBag.Id40Shift1SectionA = Id40Shift1SectionA;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift1SectionA }};
                                List<TDatarecord> data_40_Shift1_SectionA = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_40_Shift1_SectionA)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 33)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_40_Shift1_SectionA = data_40_Shift1_SectionA;

                            }

                            if (item.FSection.Equals("C"))
                            {
                                Id40Shift1SectionC = item.FShiftid;
                                ViewBag.Id40Shift1SectionC = Id40Shift1SectionC;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift1SectionC }};
                                List<TDatarecord> data_40_Shift1_SectionC = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_40_Shift1_SectionC)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 42)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_40_Shift1_SectionC = data_40_Shift1_SectionC;

                            }

                            if (item.FSection.Equals("None"))
                            {
                                Id40Shift1Prod = item.FShiftid;
                                ViewBag.Id40Shift1Prod = Id40Shift1Prod;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift1Prod }};
                                List<TDatarecord> data_40_Shift1_Prod = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                ViewBag.data_40_Shift1_Prod = data_40_Shift1_Prod;

                            }


                        }
                        if ((item.FHall.Equals("Hall 4.0")) && (item.FShift.Equals("Layer 2")))
                        {

                            if (item.FSection.Equals("A"))
                            {
                                Id40Shift2SectionA = item.FShiftid;
                                ViewBag.Id40Shift2SectionA = Id40Shift2SectionA;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift2SectionA }};
                                List<TDatarecord> data_40_Shift2_SectionA = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_40_Shift2_SectionA)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 34)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_40_Shift2_SectionA = data_40_Shift2_SectionA;

                            }
                            if (item.FSection.Equals("C"))
                            {
                                Id40Shift2SectionC = item.FShiftid;
                                ViewBag.Id40Shift2SectionC = Id40Shift2SectionC;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift2SectionC }};
                                List<TDatarecord> data_40_Shift2_SectionC = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_40_Shift2_SectionC)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 44)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_40_Shift2_SectionC = data_40_Shift2_SectionC;

                            }

                            if (item.FSection.Equals("None"))
                            {
                                Id40Shift2Prod = item.FShiftid;
                                ViewBag.Id40Shift2Prod = Id40Shift2Prod;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift2Prod }};
                                List<TDatarecord> data_40_Shift2_Prod = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                ViewBag.data_40_Shift2_Prod = data_40_Shift2_Prod;

                            }

                        }
                        if ((item.FHall.Equals("Hall 4.0")) && (item.FShift.Equals("Layer 3")))
                        {
                            if (item.FSection.Equals("A"))
                            {
                                Id40Shift3SectionA = item.FShiftid;
                                ViewBag.Id40Shift3SectionA = Id40Shift3SectionA;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift3SectionA }};
                                List<TDatarecord> data_40_Shift3_SectionA = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_40_Shift3_SectionA)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 35)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_40_Shift3_SectionA = data_40_Shift3_SectionA;

                            }

                            if (item.FSection.Equals("C"))
                            {
                                Id40Shift3SectionC = item.FShiftid;
                                ViewBag.Id40Shift3SectionC = Id40Shift3SectionC;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift3SectionC }};
                                List<TDatarecord> data_40_Shift3_SectionC = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_40_Shift3_SectionC)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 45)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_40_Shift3_SectionC = data_40_Shift3_SectionC;

                            }

                            if (item.FSection.Equals("None"))
                            {
                                Id40Shift3Prod = item.FShiftid;
                                ViewBag.Id40Shift3Prod = Id40Shift3Prod;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id40Shift3Prod }};
                                List<TDatarecord> data_40_Shift3_Prod = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                ViewBag.data_40_Shift3_Prod = data_40_Shift3_Prod;

                            }

                        }

                        /*************HALL 4.1****************/

                        if ((item.FHall.Equals("Hall 4.1")) && (item.FShift.Equals("Layer 1")))
                        {
                            if (item.FSection.Equals("A"))
                            {
                                Id41Shift1SectionA = item.FShiftid;
                                ViewBag.Id41Shift1SectionA = Id41Shift1SectionA;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift1SectionA }};
                                List<TDatarecord> data_41_Shift1_SectionA = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_41_Shift1_SectionA)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 36)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_41_Shift1_SectionA = data_41_Shift1_SectionA;

                            }
                            if (item.FSection.Equals("C"))
                            {
                                Id41Shift1SectionC = item.FShiftid;
                                ViewBag.Id41Shift1SectionC = Id41Shift1SectionC;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift1SectionC }};
                                List<TDatarecord> data_41_Shift1_SectionC = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_41_Shift1_SectionC)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 46)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_41_Shift1_SectionC = data_41_Shift1_SectionC;

                            }

                            if (item.FSection.Equals("None"))
                            {
                                Id41Shift1Prod = item.FShiftid;
                                ViewBag.Id41Shift1Prod = Id41Shift1Prod;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift1Prod }};
                                List<TDatarecord> data_41_Shift1_Prod = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                ViewBag.data_41_Shift1_Prod = data_41_Shift1_Prod;

                            }

                        }

                        if ((item.FHall.Equals("Hall 4.1")) && (item.FShift.Equals("Layer 2")))
                        {
                            if (item.FSection.Equals("A"))
                            {
                                Id41Shift2SectionA = item.FShiftid;
                                ViewBag.Id41Shift2SectionA = Id41Shift2SectionA;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift2SectionA }};
                                List<TDatarecord> data_41_Shift2_SectionA = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_41_Shift2_SectionA)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 37)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_41_Shift2_SectionA = data_41_Shift2_SectionA;
                            }
                            if (item.FSection.Equals("C"))
                            {
                                Id41Shift2SectionC = item.FShiftid;
                                ViewBag.Id41Shift2SectionC = Id41Shift2SectionC;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift2SectionC }};
                                List<TDatarecord> data_41_Shift2_SectionC = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_41_Shift2_SectionC)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 47)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_41_Shift2_SectionC = data_41_Shift2_SectionC;

                            }

                            if (item.FSection.Equals("None"))
                            {
                                Id41Shift2Prod = item.FShiftid;
                                ViewBag.Id41Shift2Prod = Id41Shift2Prod;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift2Prod }};
                                List<TDatarecord> data_41_Shift2_Prod = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                ViewBag.data_41_Shift2_Prod = data_41_Shift2_Prod;

                            }

                        }

                        if ((item.FHall.Equals("Hall 4.1")) && (item.FShift.Equals("Layer 3")))
                        {
                            if (item.FSection.Equals("A"))
                            {
                                Id41Shift3SectionA = item.FShiftid;
                                ViewBag.Id41Shift3SectionA = Id41Shift3SectionA;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift3SectionA }};
                                List<TDatarecord> data_41_Shift3_SectionA = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_41_Shift3_SectionA)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 38)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_41_Shift3_SectionA = data_41_Shift3_SectionA;

                            }
                            if (item.FSection.Equals("C"))
                            {
                                Id41Shift3SectionC = item.FShiftid;
                                ViewBag.Id41Shift3SectionC = Id41Shift3SectionC;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift3SectionC }};
                                List<TDatarecord> data_41_Shift3_SectionC = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                foreach (var elt in data_41_Shift3_SectionC)
                                {
                                    if (elt.FRecordtypeid == 1)
                                    {
                                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, date1) == 0) && (s.FTypeid == 48)).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            elt.FRecordvalue = temp.FValue;
                                        }
                                    }
                                }
                                ViewBag.data_41_Shift3_SectionC = data_41_Shift3_SectionC;
                            }

                            if (item.FSection.Equals("None"))
                            {
                                Id41Shift3Prod = item.FShiftid;
                                ViewBag.Id41Shift3Prod = Id41Shift3Prod;

                                string sql = "EXEC dbo.Get_Data @ShiftId";
                                List<SqlParameter> parms = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@ShiftId", Value = Id41Shift3Prod }};
                                List<TDatarecord> data_41_Shift3_Prod = _ctx2.TDatarecords.FromSqlRaw<TDatarecord>(sql, parms.ToArray()).ToList();
                                ViewBag.data_41_Shift3_Prod = data_41_Shift3_Prod;

                            }

                        }
                    }
                }
                ViewBag.listShiftToViewData = listShiftIdViewData;
                ViewBag.DateSelected = date1.ToLongDateString();
            }
            ViewBag.ErrorExist = _ctx2.TTranslations.Where(s => (s.FLabelid == 5) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Prod40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 6) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Prod41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 7) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ProdAMG = _ctx2.TTranslations.Where(s => (s.FLabelid == 8) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Shift = _ctx2.TTranslations.Where(s => (s.FLabelid == 9) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.SOLLProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 10) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ISTProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 11) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.TL = _ctx2.TTranslations.Where(s => (s.FLabelid == 12) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.PD = _ctx2.TTranslations.Where(s => (s.FLabelid == 13) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.NewMA = _ctx2.TTranslations.Where(s => (s.FLabelid == 14) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.SpeMeas = _ctx2.TTranslations.Where(s => (s.FLabelid == 15) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.AS = _ctx2.TTranslations.Where(s => (s.FLabelid == 16) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.TrainerZNA = _ctx2.TTranslations.Where(s => (s.FLabelid == 17) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Vac = _ctx2.TTranslations.Where(s => (s.FLabelid == 18) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.VacIND = _ctx2.TTranslations.Where(s => (s.FLabelid == 19) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Sick = _ctx2.TTranslations.Where(s => (s.FLabelid == 20) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.TempW = _ctx2.TTranslations.Where(s => (s.FLabelid == 21) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.SumPerSec = _ctx2.TTranslations.Where(s => (s.FLabelid == 22) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Sec = _ctx2.TTranslations.Where(s => (s.FLabelid == 23) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Prod90 = _ctx2.TTranslations.Where(s => (s.FLabelid == 24) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ZNASecA = _ctx2.TTranslations.Where(s => (s.FLabelid == 25) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ZNASecC = _ctx2.TTranslations.Where(s => (s.FLabelid == 26) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.TotSets = _ctx2.TTranslations.Where(s => (s.FLabelid == 27) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.BuffLev = _ctx2.TTranslations.Where(s => (s.FLabelid == 28) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.TeamDiss = _ctx2.TTranslations.Where(s => (s.FLabelid == 29) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ShiftDurBreak = _ctx2.TTranslations.Where(s => (s.FLabelid == 30) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.CycleTimeTargActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 31) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ZQB = _ctx2.TTranslations.Where(s => (s.FLabelid == 32) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.SKD = _ctx2.TTranslations.Where(s => (s.FLabelid == 33) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Valmet = _ctx2.TTranslations.Where(s => (s.FLabelid == 34) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Sindel = _ctx2.TTranslations.Where(s => (s.FLabelid == 35) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Rastatt = _ctx2.TTranslations.Where(s => (s.FLabelid == 36) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.RA41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 37) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Kecs = _ctx2.TTranslations.Where(s => (s.FLabelid == 38) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.RA40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 39) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.CycleTimeTarg = _ctx2.TTranslations.Where(s => (s.FLabelid == 40) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.CycleTimeActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 41) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ShiftDur = _ctx2.TTranslations.Where(s => (s.FLabelid == 42) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Break = _ctx2.TTranslations.Where(s => (s.FLabelid == 43) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.SOLLGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 44) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.ISTGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 45) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.SumProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 46) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Edit = _ctx2.TTranslations.Where(s => (s.FLabelid == 75) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();



            return View();
        }



        //UPDATE ON THE DATABASE
        [HttpPost]
        public IActionResult EditDataRecordAMGPost(double TEAMLEADER1, double TEAMLEADER2,
            double TEAMLEADER3, double PRESENTDIRECT1, double PRESENTDIRECT2, double PRESENTDIRECT3,
            double NEWMA1, double NEWMA2, double NEWMA3, double SPEMEAS1, double SPEMEAS2,
            double SPEMEAS3, double ANDSPRI1, double ANDSPRI2, double ANDSPRI3, double TRAIN1,
            double TRAIN2, double TRAIN3, double VACA1, double VACA2,
            double VACA3, double VACA1IND, double VACA2IND, double VACA3IND, double SICK1,
            double SICK2, double SICK3, double TEMP1, double TEMP2, double TEMP3,
            double SECTIONA1, double SECTIONA2, double SECTIONA3, double RA411,
            double RA412, double RA413, double VAL1, double VAL2, double VAL3,
            double KECS1, double KECS2, double KECS3, double RA401, double RA402,
            double RA403, string CYCLETARG1, string CYCLETARG2, string CYCLETARG3,
            string TEAMDISS1, string TEAMDISS2, string TEAMDISS3, string SHIFTDUR1, string SHIFTDUR2,
            string SHIFTDUR3, string BREAK1, string BREAK2, string BREAK3, double ZQB1,
            double ZQB2, double ZQB3)
        {
            if (TEAMDISS1 == null)
            {
                TEAMDISS1 = "0";
            }
            if (TEAMDISS2 == null)
            {
                TEAMDISS2 = "0";
            }
            if (TEAMDISS3 == null)
            {
                TEAMDISS3 = "0";
            }
            if (SHIFTDUR1 == null)
            {
                SHIFTDUR1 = "0";
            }
            if (SHIFTDUR2 == null)
            {
                SHIFTDUR2 = "0";
            }
            if (SHIFTDUR3 == null)
            {
                SHIFTDUR3 = "0";
            }
            if (BREAK1 == null)
            {
                BREAK1 = "0";
            }
            if (BREAK2 == null)
            {
                BREAK2 = "0";
            }
            if (BREAK3 == null)
            {
                BREAK3 = "0";
            }
            if (CYCLETARG1 == null)
            {
                CYCLETARG1 = "0";
            }
            if (CYCLETARG2 == null)
            {
                CYCLETARG2 = "0";
            }
            if (CYCLETARG3 == null)
            {
                CYCLETARG3 = "0";
            }





            List<double> param = new List<double>();
            param.Add(TEAMLEADER1); param.Add(PRESENTDIRECT1); param.Add(NEWMA1); param.Add(SPEMEAS1);
            param.Add(ANDSPRI1); param.Add(TRAIN1); param.Add(VACA1); param.Add(VACA1IND); param.Add(SICK1);
            param.Add(TEMP1); param.Add(SECTIONA1); param.Add(RA411); param.Add(VAL1); param.Add(KECS1);
            param.Add(RA401); param.Add(Convert.ToDouble(CYCLETARG1.Replace(".", ","))); param.Add(Convert.ToDouble(TEAMDISS1.Replace(".", ",")));
            param.Add(Convert.ToDouble(SHIFTDUR1.Replace(".", ","))); param.Add(Convert.ToDouble(BREAK1.Replace(".", ","))); param.Add(ZQB1);


            param.Add(TEAMLEADER2); param.Add(PRESENTDIRECT2); param.Add(NEWMA2); param.Add(SPEMEAS2); param.Add(ANDSPRI2);
            param.Add(TRAIN2); param.Add(VACA2); param.Add(VACA2IND); param.Add(SICK2); param.Add(TEMP2); param.Add(SECTIONA2);
            param.Add(RA412); param.Add(VAL2); param.Add(KECS2); param.Add(RA402); 
            param.Add(Convert.ToDouble(CYCLETARG2.Replace(".", ","))); param.Add(Convert.ToDouble(TEAMDISS2.Replace(".", ",")));
            param.Add(Convert.ToDouble(SHIFTDUR2.Replace(".", ","))); param.Add(Convert.ToDouble(BREAK2.Replace(".", ","))); param.Add(ZQB2);


            param.Add(TEAMLEADER3); param.Add(PRESENTDIRECT3); param.Add(NEWMA3); param.Add(SPEMEAS3); param.Add(ANDSPRI3);
            param.Add(TRAIN3); param.Add(VACA3); param.Add(VACA3IND); param.Add(SICK3); param.Add(TEMP3); param.Add(SECTIONA3);
            param.Add(RA413); param.Add(VAL3); param.Add(KECS3); param.Add(RA403);
            param.Add(Convert.ToDouble(CYCLETARG3.Replace(".", ","))); param.Add(Convert.ToDouble(TEAMDISS3.Replace(".", ",")));
            param.Add(Convert.ToDouble(SHIFTDUR3.Replace(".", ","))); param.Add(Convert.ToDouble(BREAK3.Replace(".", ","))); param.Add(ZQB3);

            List<int> shiftId = new List<int>();
            List<TShift> shiftToEdit1 = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall AMG"))).ToList();
            List<TShift> shiftToEdit = shiftToEdit1.Where(s => (DateTime.Compare(s.FDate, DateSelected) == 0)).ToList();
            foreach (var elt in shiftToEdit)
            {
                shiftId.Add(elt.FShiftid);
            }

            List<int> typeid = new List<int> { 3, 4, 13, 5, 12, 6, 9, 7, 10, 11, 16, 25, 26, 27, 28, 19, 18, 21, 22, 29 };

            for (int i = 0; i < 60; i++)
            {
                if (i <= 19)
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[0]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[0]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i <= 39) && (20 <= i))
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[1]) && ((s.FRecordtypeid == typeid[i - 20]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i-20]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[1]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if (i >= 40)
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[2]) && ((s.FRecordtypeid == typeid[i - 40]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i-40]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[2]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
            }
            return RedirectToAction("RefreshDbAMG");
        }




        //UPDATE ON THE DATABASE
        public IActionResult RefreshDbAMG()
        {
            List<int> shiftId = new List<int>();
            List<TShift> shiftToEdit1 = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall AMG"))).ToList();
            List<TShift> shiftToEdit = shiftToEdit1.Where(s => (DateTime.Compare(s.FDate, DateSelected) == 0)).ToList();
            foreach (var elt in shiftToEdit)
            {
                shiftId.Add(elt.FShiftid);
            }

            List<int> typeidToRefreshOrCreate = new List<int> { 1, 2, 23, 24, 15 };
            foreach (var id in shiftId)
            {
                foreach (var elt in typeidToRefreshOrCreate)
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        if (elt == 2)
                        {
                            TDatarecord value_PresentDirect = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                            TDatarecord value_SpeMeas = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 5)).FirstOrDefault();
                            TDatarecord value_AndonSpringer = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 12)).FirstOrDefault();
                            TDatarecord value_TrainerZNA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 6)).FirstOrDefault();
                            TDatarecord value_Temp = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();

                            double total = value_PresentDirect.FRecordvalue + value_SpeMeas.FRecordvalue + value_AndonSpringer.FRecordvalue + value_TrainerZNA.FRecordvalue + value_Temp.FRecordvalue;

                            string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                            List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = total }};
                            int rowsAffected15;
                            rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                        }

                        if (elt == 23)
                        {
                            TDatarecord value_PresentDirect = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                            TDatarecord value_Temp = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                            double value_SumPerLayer = value_PresentDirect.FRecordvalue + value_Temp.FRecordvalue;

                            string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                            List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = value_SumPerLayer }};
                            int rowsAffected15;
                            rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                        }

                        if (elt == 24)
                        {
                            TDatarecord value_Shift_Duration = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 21)).FirstOrDefault();
                            TDatarecord value_TeamDiss = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 18)).FirstOrDefault();
                            TDatarecord value_RA41 = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 25)).FirstOrDefault();
                            TDatarecord value_Valmet = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 26)).FirstOrDefault();
                            TDatarecord value_KECS = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 27)).FirstOrDefault();
                            TDatarecord value_RA40 = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 28)).FirstOrDefault();

                            double totaltemp = value_RA41.FRecordvalue + value_Valmet.FRecordvalue + value_KECS.FRecordvalue + value_RA40.FRecordvalue;

                            string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                            List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = totaltemp }};
                            int rowsAffected15;
                            rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                        }

                        if (elt == 15)
                        {
                            TDatarecord value_Shift_Duration = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 21)).FirstOrDefault();
                            TDatarecord value_TeamDiss = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 18)).FirstOrDefault();
                            TDatarecord value_RA41 = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 25)).FirstOrDefault();
                            TDatarecord value_Valmet = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 26)).FirstOrDefault();
                            TDatarecord value_KECS = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 27)).FirstOrDefault();
                            TDatarecord value_RA40 = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 28)).FirstOrDefault();

                            double totaltemp = value_RA41.FRecordvalue + value_Valmet.FRecordvalue + value_KECS.FRecordvalue + value_RA40.FRecordvalue;

                            if (totaltemp == 0)
                            {
                                string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = 0 }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }

                            else
                            {
                                double valuefinal = (value_Shift_Duration.FRecordvalue - value_TeamDiss.FRecordvalue) * 3600 / (totaltemp);
                                string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = valuefinal }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }

                        }
                    }
                }
            }
            return RedirectToAction("ViewData", new { DaySelected = DateSelected.Day, MonthSelected = DateSelected.Month, YearSelected = DateSelected.Year });
        }



        /*EDIT HALL 40*/

        //UPDATE ON THE DATABASE
        [HttpPost]
        public IActionResult EditDataRecord40Post(int TEAMLEADER1A, int TEAMLEADER1C, int TEAMLEADER2A, int TEAMLEADER2C, int TEAMLEADER3A, int TEAMLEADER3C,
            int PRESENTDIRECT1A, int PRESENTDIRECT1C, int PRESENTDIRECT2A, int PRESENTDIRECT2C, int PRESENTDIRECT3A, int PRESENTDIRECT3C,
            int NEWMA1A, int NEWMA1C, int NEWMA2A, int NEWMA2C, int NEWMA3A, int NEWMA3C,
            int SPEMEAS1A, int SPEMEAS1C, int SPEMEAS2A, int SPEMEAS2C, int SPEMEAS3A, int SPEMEAS3C,
            int ANDSPRI1A, int ANDSPRI1C, int ANDSPRI2A, int ANDSPRI2C, int ANDSPRI3A, int ANDSPRI3C,
            int TRAIN1A, int TRAIN1C, int TRAIN2A, int TRAIN2C, int TRAIN3A, int TRAIN3C,
            int VACA1A, int VACA1C, int VACA2A, int VACA2C, int VACA3A, int VACA3C,
            int VACA1AIND, int VACA1CIND, int VACA2AIND, int VACA2CIND, int VACA3AIND, int VACA3CIND,
            int SICK1A, int SICK1C, int SICK2A, int SICK2C, int SICK3A, int SICK3C,
            int TEMP1A, int TEMP1C, int TEMP2A, int TEMP2C, int TEMP3A, int TEMP3C,
            int SECTION1A, int SECTION1C, int SECTION2A, int SECTION2C, int SECTION3A, int SECTION3C,
            int TOTSETS1, int TOTSETS2, int TOTSETS3,
            int BUFFLEV1, int BUFFLEV2, int BUFFLEV3,
            string TEAMDISS1, string TEAMDISS2, string TEAMDISS3,
            string SHIFTDUR1, string SHIFTDUR2, string SHIFTDUR3,
            string BREAK1, string BREAK2, string BREAK3,
            string CYCLETARG1, string CYCLETARG2, string CYCLETARG3,
            int ZQB1, int ZQB2, int ZQB3)

        {
            if (TEAMDISS1 == null)
            {
                TEAMDISS1 = "0";
            }
            if (TEAMDISS2 == null)
            {
                TEAMDISS2 = "0";
            }
            if (TEAMDISS3 == null)
            {
                TEAMDISS3 = "0";
            }
            if (SHIFTDUR1 == null)
            {
                SHIFTDUR1 = "0";
            }
            if (SHIFTDUR2 == null)
            {
                SHIFTDUR2 = "0";
            }
            if (SHIFTDUR3 == null)
            {
                SHIFTDUR3 = "0";
            }
            if (BREAK1 == null)
            {
                BREAK1 = "0";
            }
            if (BREAK2 == null)
            {
                BREAK2 = "0";
            }
            if (BREAK3 == null)
            {
                BREAK3 = "0";
            }
            if (CYCLETARG1 == null)
            {
                CYCLETARG1 = "0";
            }
            if (CYCLETARG2 == null)
            {
                CYCLETARG2 = "0";
            }
            if (CYCLETARG3 == null)
            {
                CYCLETARG3 = "0";
            }

            List<double> param = new List<double>();

            /*layer 1 section NONE*/
            param.Add(SECTION1A); param.Add(SECTION1C); param.Add(TOTSETS1); param.Add(BUFFLEV1); param.Add( Convert.ToDouble(TEAMDISS1.Replace(".",","))); param.Add(Convert.ToDouble(SHIFTDUR1.Replace(".", ",")));
            param.Add(Convert.ToDouble(BREAK1.Replace(".", ","))); param.Add(Convert.ToDouble(CYCLETARG1.Replace(".", ","))); param.Add(ZQB1);
            /*layer 1 section A*/
            param.Add(TEAMLEADER1A); param.Add(PRESENTDIRECT1A); param.Add(NEWMA1A); param.Add(SPEMEAS1A);
            param.Add(ANDSPRI1A); param.Add(TRAIN1A); param.Add(VACA1A); param.Add(VACA1AIND); param.Add(SICK1A);
            param.Add(TEMP1A);
            /*layer 1 section C*/
            param.Add(TEAMLEADER1C); param.Add(PRESENTDIRECT1C); param.Add(NEWMA1C); param.Add(SPEMEAS1C);
            param.Add(ANDSPRI1C); param.Add(TRAIN1C); param.Add(VACA1C); param.Add(VACA1CIND); param.Add(SICK1C);
            param.Add(TEMP1C);
            /*layer 2 section NONE*/
            param.Add(SECTION2A); param.Add(SECTION2C); param.Add(TOTSETS2); param.Add(BUFFLEV2); param.Add(Convert.ToDouble(TEAMDISS2.Replace(".", ",")))
                ; param.Add(Convert.ToDouble(SHIFTDUR2.Replace(".", ",")));
            param.Add(Convert.ToDouble(BREAK2.Replace(".", ","))); param.Add(Convert.ToDouble(CYCLETARG2.Replace(".", ","))); param.Add(ZQB2);
            /*layer 2 section A*/
            param.Add(TEAMLEADER2A); param.Add(PRESENTDIRECT2A); param.Add(NEWMA2A); param.Add(SPEMEAS2A);
            param.Add(ANDSPRI2A); param.Add(TRAIN2A); param.Add(VACA2A); param.Add(VACA2AIND); param.Add(SICK2A);
            param.Add(TEMP2A);
            /*layer 2 section C*/
            param.Add(TEAMLEADER2C); param.Add(PRESENTDIRECT2C); param.Add(NEWMA2C); param.Add(SPEMEAS2C);
            param.Add(ANDSPRI2C); param.Add(TRAIN2C); param.Add(VACA2C); param.Add(VACA2CIND); param.Add(SICK2C);
            param.Add(TEMP2C);
            /*layer 3 section NONE*/
            param.Add(SECTION3A); param.Add(SECTION3C); param.Add(TOTSETS3); param.Add(BUFFLEV3); param.Add(Convert.ToDouble(TEAMDISS3.Replace(".", ",")))
                ; param.Add(Convert.ToDouble(SHIFTDUR3.Replace(".", ",")));
            param.Add(Convert.ToDouble(BREAK3.Replace(".", ","))); param.Add(Convert.ToDouble(CYCLETARG3.Replace(".", ","))); param.Add(ZQB3);
            /*layer 3 section A*/
            param.Add(TEAMLEADER3A); param.Add(PRESENTDIRECT3A); param.Add(NEWMA3A); param.Add(SPEMEAS3A);
            param.Add(ANDSPRI3A); param.Add(TRAIN3A); param.Add(VACA3A); param.Add(VACA3AIND); param.Add(SICK3A);
            param.Add(TEMP3A);
            /*layer 3 section C*/
            param.Add(TEAMLEADER3C); param.Add(PRESENTDIRECT3C); param.Add(NEWMA3C); param.Add(SPEMEAS3C);
            param.Add(ANDSPRI3C); param.Add(TRAIN3C); param.Add(VACA3C); param.Add(VACA3CIND); param.Add(SICK3C);
            param.Add(TEMP3C);





            List<int> shiftId = new List<int>();
            List<TShift> shiftToEdit1 = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall 4.0"))).ToList();
            List<TShift> shiftToEdit = shiftToEdit1.Where(s=>(DateTime.Compare(s.FDate, DateSelected) == 0)).ToList();
            foreach (var elt in shiftToEdit)
            {
                shiftId.Add(elt.FShiftid);
            }

            List<int> typeid = new List<int> {
                16, 17, 24, 20, 18, 21, 22, 19, 29,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,/* Layer 1*/
                 
                16, 17, 24, 20, 18, 21, 22, 19, 29,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11, /*Layer 2*/
                 
                16, 17, 24, 20, 18, 21, 22, 19, 29,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11};/*Layer 3*/


            for (int i = 0; i < 87; i++)
            {
                if (i <= 8)/*Prod Layer 1 */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[0]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[0]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 8) && (i <= 18))/*Layer 1 SectionA*/
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[1]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[1]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 18) && (i <= 28))//*Layer 1 SectionC */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[2]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[2]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 28) && (i <= 37))//*Layer 2 Prod */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[3]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[3]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 37) && (i <= 47))//*Layer 2 SectionA */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[4]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[4]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 47) && (i <= 57))//*Layer 2 SectionC */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[5]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[5]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 57) && (i <= 66))//*Layer 3 Prod */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[6]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[6]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 66) && (i <= 76))//*Layer 3 SectionA */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[7]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[7]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 76) && (i <= 86))//*Layer 3 SectionC */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[8]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[8]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
            }

            return RedirectToAction("RefreshDb40");
        }


        //UPDATE ON THE DATABASE
        public IActionResult RefreshDb40()
        {
            List<int> shiftId = new List<int>();
            List<TShift> shiftToEdit1 = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall 4.0"))).ToList();
            List<TShift> shiftToEdit = shiftToEdit1.Where(s => (DateTime.Compare(s.FDate, DateSelected) == 0)).ToList();
            foreach (var elt in shiftToEdit)
            {
                shiftId.Add(elt.FShiftid);
            }

            List<int> typeidToRefreshOrCreateProd = new List<int> { 15 };
            List<int> typeidToRefreshOrCreateSection = new List<int> { 1, 2, 23 };
            foreach (var id in shiftId)
            {
                TShift shiftConcerned = _ctx2.TShifts.Where(s => s.FShiftid == id).FirstOrDefault();

                if (shiftConcerned.FSection.Equals("A"))
                {
                    foreach (var elt in typeidToRefreshOrCreateSection)
                    {
                        TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                        if (existOrNot == null)
                        {
                            string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                            int rowsAffected;
                            rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                        }
                        else
                        {
                            if (elt == 2)
                            {
                                TShift SectionC = new TShift();
                                foreach (var shiftid in shiftId)
                                {
                                    TShift test = _ctx2.TShifts.Where(s => s.FShiftid == shiftid).FirstOrDefault();
                                    if ((test.FSection.Equals("C")) && (test.FShift.Equals(shiftConcerned.FShift)))
                                    {
                                        SectionC = test;
                                    }
                                }


                                TDatarecord value_PresentDirectA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_PresentDirectC = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionC.FShiftid) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_TempC = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionC.FShiftid) && (s.FTypeid == 11)).FirstOrDefault();

                                double total = value_PresentDirectA.FRecordvalue + value_PresentDirectC.FRecordvalue + value_TempA.FRecordvalue + value_TempC.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = total }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }

                            if (elt == 23)
                            {
                                TDatarecord value_PresentDirectA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                double value_SumPerLayer = value_PresentDirectA.FRecordvalue + value_TempA.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = value_SumPerLayer }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }
                        }
                    }
                }


                if (shiftConcerned.FSection.Equals("C"))
                {
                    foreach (var elt in typeidToRefreshOrCreateSection)
                    {
                        TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                        if (existOrNot == null)
                        {
                            string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                            int rowsAffected;
                            rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                        }
                        else
                        {
                            if (elt == 2)
                            {
                                TShift SectionA = new TShift();
                                foreach (var shiftid in shiftId)
                                {
                                    TShift test = _ctx2.TShifts.Where(s => s.FShiftid == shiftid).FirstOrDefault();
                                    if ((test.FSection.Equals("A")) && (test.FShift.Equals(shiftConcerned.FShift)))
                                    {
                                        SectionA = test;
                                    }
                                }

                                TDatarecord value_PresentDirectA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_PresentDirectC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_TempC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_SpeMeasA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 5)).FirstOrDefault();
                                TDatarecord value_SpeMeasC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 5)).FirstOrDefault();
                                TDatarecord value_AndonSpringerA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 12)).FirstOrDefault();
                                TDatarecord value_AndonSpringerC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 12)).FirstOrDefault();
                                TDatarecord value_TrainerZNAA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 6)).FirstOrDefault();
                                TDatarecord value_TrainerZNAC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 6)).FirstOrDefault();

                                double total = value_PresentDirectA.FRecordvalue + value_PresentDirectC.FRecordvalue + value_TempA.FRecordvalue + value_TempC.FRecordvalue + value_SpeMeasA.FRecordvalue + value_SpeMeasC.FRecordvalue + value_AndonSpringerA.FRecordvalue + value_AndonSpringerC.FRecordvalue + value_TrainerZNAA.FRecordvalue + value_TrainerZNAC.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = total }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }

                            if (elt == 23)
                            {

                                TDatarecord value_PresentDirectC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                double value_SumPerLayer = value_PresentDirectC.FRecordvalue + value_TempC.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = value_SumPerLayer }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }
                        }
                    }
                }
                if (shiftConcerned.FSection.Equals("None"))
                {
                    foreach (var elt in typeidToRefreshOrCreateProd)
                    {
                        TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                        if (existOrNot == null)
                        {
                            string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                            int rowsAffected;
                            rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                        }
                        else
                        {
                            if (elt == 15)
                            {
                                TDatarecord value_Shift_Duration = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 21)).FirstOrDefault();
                                TDatarecord value_TeamDiss = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 18)).FirstOrDefault();
                                TDatarecord value_TotalSets = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 24)).FirstOrDefault();
                                double totaltemp = value_TotalSets.FRecordvalue;

                                if (totaltemp == 0)
                                {
                                    string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                    List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = 0 }};
                                    int rowsAffected15;
                                    rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                                }

                                else
                                {
                                    double valuefinal = (value_Shift_Duration.FRecordvalue - value_TeamDiss.FRecordvalue) * 3600 / (totaltemp);
                                    string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                    List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = valuefinal }};
                                    int rowsAffected15;
                                    rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                                }

                            }
                        }
                    }
                }
            }
            return RedirectToAction("ViewData", new { DaySelected = DateSelected.Day, MonthSelected = DateSelected.Month, YearSelected = DateSelected.Year });
        }

        /*EDIT HALL 41*/


        //UPDATE ON THE DATABASE

        [HttpPost]
        public IActionResult EditDataRecord41Post(int TEAMLEADER1A, int TEAMLEADER1C, int TEAMLEADER2A, int TEAMLEADER2C, int TEAMLEADER3A, int TEAMLEADER3C,
            int PRESENTDIRECT1A, int PRESENTDIRECT1C, int PRESENTDIRECT2A, int PRESENTDIRECT2C, int PRESENTDIRECT3A, int PRESENTDIRECT3C,
            int NEWMA1A, int NEWMA1C, int NEWMA2A, int NEWMA2C, int NEWMA3A, int NEWMA3C,
            int SPEMEAS1A, int SPEMEAS1C, int SPEMEAS2A, int SPEMEAS2C, int SPEMEAS3A, int SPEMEAS3C,
            int ANDSPRI1A, int ANDSPRI1C, int ANDSPRI2A, int ANDSPRI2C, int ANDSPRI3A, int ANDSPRI3C,
            int TRAIN1A, int TRAIN1C, int TRAIN2A, int TRAIN2C, int TRAIN3A, int TRAIN3C,
            int VACA1A, int VACA1C, int VACA2A, int VACA2C, int VACA3A, int VACA3C,
            int VACA1AIND, int VACA1CIND, int VACA2AIND, int VACA2CIND, int VACA3AIND, int VACA3CIND,
            int SICK1A, int SICK1C, int SICK2A, int SICK2C, int SICK3A, int SICK3C,
            int TEMP1A, int TEMP1C, int TEMP2A, int TEMP2C, int TEMP3A, int TEMP3C,
            int SECTION1A, int SECTION1C, int SECTION2A, int SECTION2C, int SECTION3A, int SECTION3C,
            int SKD1, int SKD2, int SKD3,
            int VAL1, int VAL2, int VAL3,
            int SIN1, int SIN2, int SIN3,
            int RAS1, int RAS2, int RAS3,
            int BUFFLEV1, int BUFFLEV2, int BUFFLEV3,
            string TEAMDISS1, string TEAMDISS2, string TEAMDISS3,
            string SHIFTDUR1, string SHIFTDUR2, string SHIFTDUR3,
            string BREAK1, string BREAK2, string BREAK3,
            string CYCLETARG1, string CYCLETARG2, string CYCLETARG3,
            int ZQB1, int ZQB2, int ZQB3)

        {
            if (TEAMDISS1 == null)
            {
                TEAMDISS1 = "0";
            }
            if (TEAMDISS2 == null)
            {
                TEAMDISS2 = "0";
            }
            if (TEAMDISS3 == null)
            {
                TEAMDISS3 = "0";
            }
            if (SHIFTDUR1 == null)
            {
                SHIFTDUR1 = "0";
            }
            if (SHIFTDUR2 == null)
            {
                SHIFTDUR2 = "0";
            }
            if (SHIFTDUR3 == null)
            {
                SHIFTDUR3 = "0";
            }
            if (BREAK1 == null)
            {
                BREAK1 = "0";
            }
            if (BREAK2 == null)
            {
                BREAK2 = "0";
            }
            if (BREAK3 == null)
            {
                BREAK3 = "0";
            }
            if (CYCLETARG1 == null)
            {
                CYCLETARG1 = "0";
            }
            if (CYCLETARG2 == null)
            {
                CYCLETARG2 = "0";
            }
            if (CYCLETARG3 == null)
            {
                CYCLETARG3 = "0";
            }






            List<double> param = new List<double>();

            /*layer 1 section NONE*/
            param.Add(SECTION1A); param.Add(SECTION1C); param.Add(SKD1); param.Add(VAL1); param.Add(SIN1); param.Add(RAS1); param.Add(BUFFLEV1); 
            param.Add(Convert.ToDouble(TEAMDISS1.Replace(".",","))); param.Add(Convert.ToDouble(SHIFTDUR1.Replace(".", ",")));
            param.Add(Convert.ToDouble(BREAK1.Replace(".", ","))); param.Add(Convert.ToDouble(CYCLETARG1.Replace(".", ","))); param.Add(ZQB1);
            /*layer 1 section A*/
            param.Add(TEAMLEADER1A); param.Add(PRESENTDIRECT1A); param.Add(NEWMA1A); param.Add(SPEMEAS1A);
            param.Add(ANDSPRI1A); param.Add(TRAIN1A); param.Add(VACA1A); param.Add(VACA1AIND); param.Add(SICK1A);
            param.Add(TEMP1A);
            /*layer 1 section C*/
            param.Add(TEAMLEADER1C); param.Add(PRESENTDIRECT1C); param.Add(NEWMA1C); param.Add(SPEMEAS1C);
            param.Add(ANDSPRI1C); param.Add(TRAIN1C); param.Add(VACA1C); param.Add(VACA1CIND); param.Add(SICK1C);
            param.Add(TEMP1C);
            /*layer 2 section NONE*/
            param.Add(SECTION2A); param.Add(SECTION2C); param.Add(SKD2); param.Add(VAL2); param.Add(SIN2); param.Add(RAS2); param.Add(BUFFLEV2);
            param.Add(Convert.ToDouble(TEAMDISS2.Replace(".", ","))); param.Add(Convert.ToDouble(SHIFTDUR2.Replace(".", ",")));
            param.Add(Convert.ToDouble(BREAK2.Replace(".", ","))); param.Add(Convert.ToDouble(CYCLETARG2.Replace(".", ","))); param.Add(ZQB2);
            /*layer 2 section A*/
            param.Add(TEAMLEADER2A); param.Add(PRESENTDIRECT2A); param.Add(NEWMA2A); param.Add(SPEMEAS2A);
            param.Add(ANDSPRI2A); param.Add(TRAIN2A); param.Add(VACA2A); param.Add(VACA2AIND); param.Add(SICK2A);
            param.Add(TEMP2A);
            /*layer 2 section C*/
            param.Add(TEAMLEADER2C); param.Add(PRESENTDIRECT2C); param.Add(NEWMA2C); param.Add(SPEMEAS2C);
            param.Add(ANDSPRI2C); param.Add(TRAIN2C); param.Add(VACA2C); param.Add(VACA2CIND); param.Add(SICK2C);
            param.Add(TEMP2C);
            /*layer 3 section NONE*/
            param.Add(SECTION3A); param.Add(SECTION3C); param.Add(SKD3); param.Add(VAL3); param.Add(SIN3); param.Add(RAS3); param.Add(BUFFLEV3);
            param.Add(Convert.ToDouble(TEAMDISS3.Replace(".", ","))); param.Add(Convert.ToDouble(SHIFTDUR3.Replace(".", ",")));
            param.Add(Convert.ToDouble(BREAK3.Replace(".", ","))); param.Add(Convert.ToDouble(CYCLETARG3.Replace(".", ","))); param.Add(ZQB3);
            /*layer 3 section A*/
            param.Add(TEAMLEADER3A); param.Add(PRESENTDIRECT3A); param.Add(NEWMA3A); param.Add(SPEMEAS3A);
            param.Add(ANDSPRI3A); param.Add(TRAIN3A); param.Add(VACA3A); param.Add(VACA3AIND); param.Add(SICK3A);
            param.Add(TEMP3A);
            /*layer 3 section C*/
            param.Add(TEAMLEADER3C); param.Add(PRESENTDIRECT3C); param.Add(NEWMA3C); param.Add(SPEMEAS3C);
            param.Add(ANDSPRI3C); param.Add(TRAIN3C); param.Add(VACA3C); param.Add(VACA3CIND); param.Add(SICK3C);
            param.Add(TEMP3C);


            List<int> shiftId = new List<int>();
            List<TShift> shiftToEdit1 = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall 4.1"))).ToList();
            List<TShift> shiftToEdit = shiftToEdit1.Where(s => (DateTime.Compare(s.FDate, DateSelected) == 0)).ToList();
            foreach (var elt in shiftToEdit)
            {
                shiftId.Add(elt.FShiftid);
            }

            List<int> typeid = new List<int> {
                16, 17, 31, 26, 30, 32, 20, 18, 21, 22, 19, 29,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,/* Layer 1*/
                 
                16, 17, 31, 26, 30, 32, 20, 18, 21, 22, 19, 29,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11, /*Layer 2*/
                 
                16, 17, 31, 26, 30, 32, 20, 18, 21, 22, 19, 29,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11,
                3, 4, 13, 5, 12, 6, 9, 7, 10, 11};/*Layer 3*/

            for (int i = 0; i < 96; i++)
            {
                if (i <= 11)/*Prod Layer 1 */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[0]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[0]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 11) && (i <= 21))/*Layer 1 SectionA*/
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[1]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[1]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 21) && (i <= 31))//*Layer 1 SectionC */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[2]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[2]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 31) && (i <= 43))//*Layer 2 Prod */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[3]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[3]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 43) && (i <= 53))//*Layer 2 SectionA */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[4]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[4]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 53) && (i <= 63))//*Layer 2 SectionC */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[5]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[5]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 63) && (i <= 75))//*Layer 3 Prod */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[6]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[6]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 75) && (i <= 85))//*Layer 3 SectionA */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[7]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[7]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
                if ((i > 85) && (i <= 95))//*Layer 3 SectionC */
                {
                    TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == shiftId[8]) && ((s.FRecordtypeid == typeid[i]))).FirstOrDefault();
                    if (existOrNot == null)
                    {
                        string sql = "EXEC dbo.Add_Onedatarecord @TypeId,@ShiftId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =typeid[i]},
                            new SqlParameter { ParameterName = "@ShiftId", Value =shiftId[8]},
                            new SqlParameter { ParameterName = "@Value", Value=param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                    else
                    {
                        string sql = "EXEC dbo.Edit_datarecordAMG @RecordId,@Value";
                        List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@RecordId", Value =existOrNot.FRecordid},
                            new SqlParameter { ParameterName = "@Value", Value =param[i]}};
                        int rowsAffected;
                        rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                    }
                }
            }

            return RedirectToAction("RefreshDb41");
        }


        //UPDATE ON THE DATABASE
        public IActionResult RefreshDb41()
        {
            List<int> shiftId = new List<int>();
            List<TShift> shiftToEdit1 = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall 4.1"))).ToList();
            List<TShift> shiftToEdit = shiftToEdit1.Where(s => (DateTime.Compare(s.FDate, DateSelected) == 0)).ToList();
            foreach (var elt in shiftToEdit)
            {
                shiftId.Add(elt.FShiftid);
            }

            List<int> typeidToRefreshOrCreateProd = new List<int> { 24, 15 };
            List<int> typeidToRefreshOrCreateSection = new List<int> { 1, 2, 23 };
            foreach (var id in shiftId)
            {
                TShift shiftConcerned = _ctx2.TShifts.Where(s => s.FShiftid == id).FirstOrDefault();

                if (shiftConcerned.FSection.Equals("A"))
                {
                    foreach (var elt in typeidToRefreshOrCreateSection)
                    {
                        TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                        if (existOrNot == null)
                        {
                            string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                            int rowsAffected;
                            rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                        }
                        else
                        {
                            if (elt == 2)
                            {
                                TShift SectionC = new TShift();
                                foreach (var shiftid in shiftId)
                                {
                                    TShift test = _ctx2.TShifts.Where(s => s.FShiftid == shiftid).FirstOrDefault();
                                    if ((test.FSection.Equals("C")) && (test.FShift.Equals(shiftConcerned.FShift)))
                                    {
                                        SectionC = test;
                                    }
                                }


                                TDatarecord value_PresentDirectA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_PresentDirectC = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionC.FShiftid) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_TempC = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionC.FShiftid) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_AndonSpringerA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 12)).FirstOrDefault();
                                TDatarecord value_AndonSpringerC = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionC.FShiftid) && (s.FTypeid == 12)).FirstOrDefault();
                                TDatarecord value_TrainerZNAA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 6)).FirstOrDefault();
                                TDatarecord value_TrainerZNAC = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionC.FShiftid) && (s.FTypeid == 6)).FirstOrDefault();


                                double total = value_PresentDirectA.FRecordvalue + value_PresentDirectC.FRecordvalue + value_TempA.FRecordvalue + value_TempC.FRecordvalue + value_AndonSpringerA.FRecordvalue + value_AndonSpringerC.FRecordvalue + value_TrainerZNAA.FRecordvalue + value_TrainerZNAC.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = total }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }

                            if (elt == 23)
                            {
                                TDatarecord value_PresentDirectA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempA = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                double value_SumPerLayer = value_PresentDirectA.FRecordvalue + value_TempA.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = value_SumPerLayer }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }
                        }
                    }
                }


                if (shiftConcerned.FSection.Equals("C"))
                {
                    foreach (var elt in typeidToRefreshOrCreateSection)
                    {
                        TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                        if (existOrNot == null)
                        {
                            string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                            int rowsAffected;
                            rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                        }
                        else
                        {
                            if (elt == 2)
                            {
                                TShift SectionA = new TShift();
                                foreach (var shiftid in shiftId)
                                {
                                    TShift test = _ctx2.TShifts.Where(s => s.FShiftid == shiftid).FirstOrDefault();
                                    if ((test.FSection.Equals("A")) && (test.FShift.Equals(shiftConcerned.FShift)))
                                    {
                                        SectionA = test;
                                    }
                                }

                                TDatarecord value_PresentDirectA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_PresentDirectC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_TempC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                TDatarecord value_SpeMeasA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 5)).FirstOrDefault();
                                TDatarecord value_SpeMeasC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 5)).FirstOrDefault();
                                TDatarecord value_AndonSpringerA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 12)).FirstOrDefault();
                                TDatarecord value_AndonSpringerC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 12)).FirstOrDefault();
                                TDatarecord value_TrainerZNAA = _ctx2.TDatarecords.Where(s => (s.FShiftid == SectionA.FShiftid) && (s.FTypeid == 6)).FirstOrDefault();
                                TDatarecord value_TrainerZNAC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 6)).FirstOrDefault();

                                double total = value_PresentDirectA.FRecordvalue + value_PresentDirectC.FRecordvalue + value_TempA.FRecordvalue + value_TempC.FRecordvalue + value_SpeMeasA.FRecordvalue + value_SpeMeasC.FRecordvalue + value_AndonSpringerA.FRecordvalue + value_AndonSpringerC.FRecordvalue + value_TrainerZNAA.FRecordvalue + value_TrainerZNAC.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = total }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }

                            if (elt == 23)
                            {

                                TDatarecord value_PresentDirectC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 4)).FirstOrDefault();
                                TDatarecord value_TempC = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 11)).FirstOrDefault();
                                double value_SumPerLayer = value_PresentDirectC.FRecordvalue + value_TempC.FRecordvalue;

                                string sql15 = "EXEC dbo.Refresh_datarecordAMG @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = value_SumPerLayer }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                            }
                        }
                    }
                }
                if (shiftConcerned.FSection.Equals("None"))
                {
                    foreach (var elt in typeidToRefreshOrCreateProd)
                    {
                        TDatarecord existOrNot = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && ((s.FRecordtypeid == elt))).FirstOrDefault();
                        if (existOrNot == null)
                        {
                            string sql = "EXEC dbo.Add_OnedatarecordAMG @TypeId,@ShiftId";
                            List<SqlParameter> parms = new List<SqlParameter>
                            {new SqlParameter { ParameterName = "@TypeId", Value =elt},
                            new SqlParameter { ParameterName = "@ShiftId", Value =id}};
                            int rowsAffected;
                            rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                        }
                        else
                        {

                            if (elt == 24)
                            {
                                TDatarecord value_SKDSets = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 31)).FirstOrDefault();
                                TDatarecord value_ValmetSets = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 26)).FirstOrDefault();
                                TDatarecord value_SindelSets = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 30)).FirstOrDefault();
                                TDatarecord value_RastattSets = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 32)).FirstOrDefault();
                                double totaltemp = value_SKDSets.FRecordvalue + value_ValmetSets.FRecordvalue + value_SindelSets.FRecordvalue + value_RastattSets.FRecordvalue;


                                string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = totaltemp }};
                                int rowsAffected15;
                                rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());


                            }




                            if (elt == 15)
                            {
                                TDatarecord value_Shift_Duration = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 21)).FirstOrDefault();
                                TDatarecord value_TeamDiss = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 18)).FirstOrDefault();
                                TDatarecord value_TotalSets = _ctx2.TDatarecords.Where(s => (s.FShiftid == id) && (s.FTypeid == 24)).FirstOrDefault();
                                double totaltemp = value_TotalSets.FRecordvalue;

                                if (totaltemp == 0)
                                {
                                    string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                    List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = 0 }};
                                    int rowsAffected15;
                                    rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                                }

                                else
                                {
                                    double valuefinal = (value_Shift_Duration.FRecordvalue - value_TeamDiss.FRecordvalue) * 3600 / (totaltemp);
                                    string sql15 = "EXEC dbo.Refresh_datarecordAMG_15 @FRecordId,@ShiftId,@Value";
                                    List<SqlParameter> parms15 = new List<SqlParameter>
                                {new SqlParameter { ParameterName = "@FRecordId", Value =existOrNot.FRecordid},
                                new SqlParameter { ParameterName = "@ShiftId", Value =id},
                                new SqlParameter { ParameterName = "@Value", Value = valuefinal }};
                                    int rowsAffected15;
                                    rowsAffected15 = _ctx2.Database.ExecuteSqlRaw(sql15, parms15.ToArray());
                                }

                            }




                        }
                    }
                }
            }
            return RedirectToAction("ViewData", new { DaySelected = DateSelected.Day, MonthSelected = DateSelected.Month, YearSelected = DateSelected.Year });
        }



        public IActionResult InputDataPlan()
        {

            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            AfficheBand = 0;
            ProductionSelected = null;
            ShiftSelected = null;
            SectionSelected = null;

            List<int> listYear = new List<int>();
            List<int> listYearWithoutDuplicates = new List<int>();



            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {

                ViewBag.choiceofdate = _ctx2.TTranslations.Where(s => (s.FLabelid == 66) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.datefield = _ctx2.TTranslations.Where(s => (s.FLabelid == 67) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.nextbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 68) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();





                return View();
            }

        }

        public IActionResult AddOrEditDataPlan(DateTime ReportDate)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            AfficheBand = 0;
            ProductionSelected = null;
            ShiftSelected = null;
            SectionSelected = null;

            dataPlan_dateSelected = ReportDate;

            List<int> listYear = new List<int>();
            List<int> listYearWithoutDuplicates = new List<int>();



            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                List<TDataplant> listOfDataPlan = new List<TDataplant>();
                listOfDataPlan = _ctx2.TDataplants.Where(s => DateTime.Compare(s.FDate, ReportDate) == 0).ToList();
                if (listOfDataPlan.Count == 0)
                {
                    ViewBag.date = ReportDate.Date;
                    return RedirectToAction("AddDataPlan", new { DataDate = ReportDate });
                }
                else
                {
                    return RedirectToAction("EditDataPlan", new { DataDate = ReportDate });
                }
            }
        }

        public IActionResult AddDataPlan(DateTime ReportDate)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;
            AfficheBand = 0;
            ProductionSelected = null;
            ShiftSelected = null;
            SectionSelected = null;

            List<int> listYear = new List<int>();
            List<int> listYearWithoutDuplicates = new List<int>();
            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {

                ViewBag.field1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 69) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.field2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 70) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.field3 = _ctx2.TTranslations.Where(s => (s.FLabelid == 71) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.field4 = _ctx2.TTranslations.Where(s => (s.FLabelid == 72) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.submitbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 73) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

                return View();
            }
        }



        //UPDATE ON THE DATABASE
        [HttpPost]
        public IActionResult AddDataPlanPost(int MASOLL401, int MASOLL402, int MASOLL403, int MASOLL411, int MASOLL412, int MASOLL413,
            int MASOLLAMG1, int MASOLLAMG2, int MASOLLAMG3, int MASOLLSECOND401, int MASOLLSECOND402, int MASOLLSECOND403,
            int MASOLLSECOND411, int MASOLLSECOND412, int MASOLLSECOND413, int MASOLLSECONDAMG1, int MASOLLSECONDAMG2, int MASOLLSECONDAMG3)
        {
            List<int> param = new List<int>();

            param.Add(MASOLL401); param.Add(MASOLL402); param.Add(MASOLL403);
            param.Add(MASOLL411); param.Add(MASOLL412); param.Add(MASOLL413);
            param.Add(MASOLLAMG1); param.Add(MASOLLAMG2); param.Add(MASOLLAMG3);
            param.Add(MASOLLSECOND401); param.Add(MASOLLSECOND402); param.Add(MASOLLSECOND403);
            param.Add(MASOLLSECOND411); param.Add(MASOLLSECOND412); param.Add(MASOLLSECOND413);
            param.Add(MASOLLSECONDAMG1); param.Add(MASOLLSECONDAMG2); param.Add(MASOLLSECONDAMG3);

            List<int> ListOfTypeId = new List<int> { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 44, 45, 46, 47, 48, 49, 50, 51 };

            int daysinmonth = DateTime.DaysInMonth(dataPlan_dateSelected.Year, dataPlan_dateSelected.Month);
            for (int i = 1; i <= daysinmonth; i++)
            {
                DateTime inputDate = new DateTime(dataPlan_dateSelected.Year, dataPlan_dateSelected.Month, i);
                DateSelected = inputDate;
                for (int j = 0; j <= 17; j++)
                {
                    string sql = "EXEC dbo.Add_DataPlan @Value,@TypeId,@FDate";
                    List<SqlParameter> parms = new List<SqlParameter>
                {new SqlParameter { ParameterName = "@Value", Value =param[j]},
                 new SqlParameter { ParameterName = "@TypeId", Value =ListOfTypeId[j]},
                 new SqlParameter { ParameterName = "@FDate", Value = inputDate }};
                    int rowsAffected;
                    rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                }
            }
            return RedirectToAction("InputDataPlan");
        }




        public IActionResult EditDataPlan(DateTime DataDate)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;

            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                List<List<string>> FieldsName = new List<List<string>>();
                List<List<string>> FieldSecName = new List<List<string>>();
                List<DateTime> DaysOfMonthList = new List<DateTime>();
                int daysinmonth = DateTime.DaysInMonth(dataPlan_dateSelected.Year, dataPlan_dateSelected.Month);
                for (int i = 1; i <= daysinmonth; i++)
                {
                    DateTime dateToAdd = new DateTime(dataPlan_dateSelected.Year, dataPlan_dateSelected.Month, i);
                    DaysOfMonthList.Add(dateToAdd);
                }
                for (int i = 1; i <= daysinmonth; i++)
                {
                    List<string> temp = new List<string>();
                    for (int j = 0; j <= 8; j++)
                    {
                        temp.Add("field" + i.ToString() + j.ToString());
                    }
                    FieldsName.Add(temp);
                }

                for (int i = 1; i <= daysinmonth; i++)
                {
                    List<string> temp = new List<string>();
                    for (int j = 0; j <= 8; j++)
                    {
                        temp.Add("fieldsec" + i.ToString() + j.ToString());
                    }
                    FieldSecName.Add(temp);
                }


                ViewBag.FieldsName = FieldsName;
                ViewBag.FieldSecName = FieldSecName;
                ViewBag.DaysOfDate = DaysOfMonthList;

                List<TDataplant> dataplant = _ctx2.TDataplants.Where(s => (s.FDate.Month == DataDate.Month) && (s.FDate.Year == DataDate.Year)).ToList();
                ViewBag.dataplant = dataplant;

                ViewBag.mess1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 74) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.mess2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 87) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.submitbutton = _ctx2.TTranslations.Where(s => (s.FLabelid == 73) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.datefield = _ctx2.TTranslations.Where(s => (s.FLabelid == 88) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();

                return View();
            }
        }



        //UPDATE ON THE DATABASE
        [HttpPost]
        public IActionResult EditDataPlanPost(
            int field10, int field11, int field12, int field13, int field14, int field15, int field16, int field17, int field18,
            int field20, int field21, int field22, int field23, int field24, int field25, int field26, int field27, int field28,
            int field30, int field31, int field32, int field33, int field34, int field35, int field36, int field37, int field38,
            int field40, int field41, int field42, int field43, int field44, int field45, int field46, int field47, int field48,
            int field50, int field51, int field52, int field53, int field54, int field55, int field56, int field57, int field58,
            int field60, int field61, int field62, int field63, int field64, int field65, int field66, int field67, int field68,
            int field70, int field71, int field72, int field73, int field74, int field75, int field76, int field77, int field78,
            int field80, int field81, int field82, int field83, int field84, int field85, int field86, int field87, int field88,
            int field90, int field91, int field92, int field93, int field94, int field95, int field96, int field97, int field98,
            int field100, int field101, int field102, int field103, int field104, int field105, int field106, int field107, int field108,
            int field110, int field111, int field112, int field113, int field114, int field115, int field116, int field117, int field118,
            int field120, int field121, int field122, int field123, int field124, int field125, int field126, int field127, int field128,
            int field130, int field131, int field132, int field133, int field134, int field135, int field136, int field137, int field138,
            int field140, int field141, int field142, int field143, int field144, int field145, int field146, int field147, int field148,
            int field150, int field151, int field152, int field153, int field154, int field155, int field156, int field157, int field158,
            int field160, int field161, int field162, int field163, int field164, int field165, int field166, int field167, int field168,
            int field170, int field171, int field172, int field173, int field174, int field175, int field176, int field177, int field178,
            int field180, int field181, int field182, int field183, int field184, int field185, int field186, int field187, int field188,
            int field190, int field191, int field192, int field193, int field194, int field195, int field196, int field197, int field198,
            int field200, int field201, int field202, int field203, int field204, int field205, int field206, int field207, int field208,
            int field210, int field211, int field212, int field213, int field214, int field215, int field216, int field217, int field218,
            int field220, int field221, int field222, int field223, int field224, int field225, int field226, int field227, int field228,
            int field230, int field231, int field232, int field233, int field234, int field235, int field236, int field237, int field238,
            int field240, int field241, int field242, int field243, int field244, int field245, int field246, int field247, int field248,
            int field250, int field251, int field252, int field253, int field254, int field255, int field256, int field257, int field258,
            int field260, int field261, int field262, int field263, int field264, int field265, int field266, int field267, int field268,
            int field270, int field271, int field272, int field273, int field274, int field275, int field276, int field277, int field278,
            int field280, int field281, int field282, int field283, int field284, int field285, int field286, int field287, int field288,
            int field290, int field291, int field292, int field293, int field294, int field295, int field296, int field297, int field298,
            int field300, int field301, int field302, int field303, int field304, int field305, int field306, int field307, int field308,
            int field310, int field311, int field312, int field313, int field314, int field315, int field316, int field317, int field318,
            int fieldsec10, int fieldsec11, int fieldsec12, int fieldsec13, int fieldsec14, int fieldsec15, int fieldsec16, int fieldsec17, int fieldsec18,
            int fieldsec20, int fieldsec21, int fieldsec22, int fieldsec23, int fieldsec24, int fieldsec25, int fieldsec26, int fieldsec27, int fieldsec28,
            int fieldsec30, int fieldsec31, int fieldsec32, int fieldsec33, int fieldsec34, int fieldsec35, int fieldsec36, int fieldsec37, int fieldsec38,
            int fieldsec40, int fieldsec41, int fieldsec42, int fieldsec43, int fieldsec44, int fieldsec45, int fieldsec46, int fieldsec47, int fieldsec48,
            int fieldsec50, int fieldsec51, int fieldsec52, int fieldsec53, int fieldsec54, int fieldsec55, int fieldsec56, int fieldsec57, int fieldsec58,
            int fieldsec60, int fieldsec61, int fieldsec62, int fieldsec63, int fieldsec64, int fieldsec65, int fieldsec66, int fieldsec67, int fieldsec68,
            int fieldsec70, int fieldsec71, int fieldsec72, int fieldsec73, int fieldsec74, int fieldsec75, int fieldsec76, int fieldsec77, int fieldsec78,
            int fieldsec80, int fieldsec81, int fieldsec82, int fieldsec83, int fieldsec84, int fieldsec85, int fieldsec86, int fieldsec87, int fieldsec88,
            int fieldsec90, int fieldsec91, int fieldsec92, int fieldsec93, int fieldsec94, int fieldsec95, int fieldsec96, int fieldsec97, int fieldsec98,
            int fieldsec100, int fieldsec101, int fieldsec102, int fieldsec103, int fieldsec104, int fieldsec105, int fieldsec106, int fieldsec107, int fieldsec108,
            int fieldsec110, int fieldsec111, int fieldsec112, int fieldsec113, int fieldsec114, int fieldsec115, int fieldsec116, int fieldsec117, int fieldsec118,
            int fieldsec120, int fieldsec121, int fieldsec122, int fieldsec123, int fieldsec124, int fieldsec125, int fieldsec126, int fieldsec127, int fieldsec128,
            int fieldsec130, int fieldsec131, int fieldsec132, int fieldsec133, int fieldsec134, int fieldsec135, int fieldsec136, int fieldsec137, int fieldsec138,
            int fieldsec140, int fieldsec141, int fieldsec142, int fieldsec143, int fieldsec144, int fieldsec145, int fieldsec146, int fieldsec147, int fieldsec148,
            int fieldsec150, int fieldsec151, int fieldsec152, int fieldsec153, int fieldsec154, int fieldsec155, int fieldsec156, int fieldsec157, int fieldsec158,
            int fieldsec160, int fieldsec161, int fieldsec162, int fieldsec163, int fieldsec164, int fieldsec165, int fieldsec166, int fieldsec167, int fieldsec168,
            int fieldsec170, int fieldsec171, int fieldsec172, int fieldsec173, int fieldsec174, int fieldsec175, int fieldsec176, int fieldsec177, int fieldsec178,
            int fieldsec180, int fieldsec181, int fieldsec182, int fieldsec183, int fieldsec184, int fieldsec185, int fieldsec186, int fieldsec187, int fieldsec188,
            int fieldsec190, int fieldsec191, int fieldsec192, int fieldsec193, int fieldsec194, int fieldsec195, int fieldsec196, int fieldsec197, int fieldsec198,
            int fieldsec200, int fieldsec201, int fieldsec202, int fieldsec203, int fieldsec204, int fieldsec205, int fieldsec206, int fieldsec207, int fieldsec208,
            int fieldsec210, int fieldsec211, int fieldsec212, int fieldsec213, int fieldsec214, int fieldsec215, int fieldsec216, int fieldsec217, int fieldsec218,
            int fieldsec220, int fieldsec221, int fieldsec222, int fieldsec223, int fieldsec224, int fieldsec225, int fieldsec226, int fieldsec227, int fieldsec228,
            int fieldsec230, int fieldsec231, int fieldsec232, int fieldsec233, int fieldsec234, int fieldsec235, int fieldsec236, int fieldsec237, int fieldsec238,
            int fieldsec240, int fieldsec241, int fieldsec242, int fieldsec243, int fieldsec244, int fieldsec245, int fieldsec246, int fieldsec247, int fieldsec248,
            int fieldsec250, int fieldsec251, int fieldsec252, int fieldsec253, int fieldsec254, int fieldsec255, int fieldsec256, int fieldsec257, int fieldsec258,
            int fieldsec260, int fieldsec261, int fieldsec262, int fieldsec263, int fieldsec264, int fieldsec265, int fieldsec266, int fieldsec267, int fieldsec268,
            int fieldsec270, int fieldsec271, int fieldsec272, int fieldsec273, int fieldsec274, int fieldsec275, int fieldsec276, int fieldsec277, int fieldsec278,
            int fieldsec280, int fieldsec281, int fieldsec282, int fieldsec283, int fieldsec284, int fieldsec285, int fieldsec286, int fieldsec287, int fieldsec288,
            int fieldsec290, int fieldsec291, int fieldsec292, int fieldsec293, int fieldsec294, int fieldsec295, int fieldsec296, int fieldsec297, int fieldsec298,
            int fieldsec300, int fieldsec301, int fieldsec302, int fieldsec303, int fieldsec304, int fieldsec305, int fieldsec306, int fieldsec307, int fieldsec308,
            int fieldsec310, int fieldsec311, int fieldsec312, int fieldsec313, int fieldsec314, int fieldsec315, int fieldsec316, int fieldsec317, int fieldsec318)
        {


            List<List<int>> parame = new List<List<int>>();

            List<int> parame2 = new List<int> { field10, field11, field12, field13, field14, field15, field16, field17, field18, fieldsec10, fieldsec11, fieldsec12, fieldsec13, fieldsec14, fieldsec15, fieldsec16, fieldsec17, fieldsec18 };
            List<int> parame3 = new List<int> { field20, field21, field22, field23, field24, field25, field26, field27, field28, fieldsec20, fieldsec21, fieldsec22, fieldsec23, fieldsec24, fieldsec25, fieldsec26, fieldsec27, fieldsec28 };
            List<int> parame4 = new List<int> { field30, field31, field32, field33, field34, field35, field36, field37, field38, fieldsec30, fieldsec31, fieldsec32, fieldsec33, fieldsec34, fieldsec35, fieldsec36, fieldsec37, fieldsec38 };
            List<int> parame5 = new List<int> { field40, field41, field42, field43, field44, field45, field46, field47, field48, fieldsec40, fieldsec41, fieldsec42, fieldsec43, fieldsec44, fieldsec45, fieldsec46, fieldsec47, fieldsec48 };
            List<int> parame6 = new List<int> { field50, field51, field52, field53, field54, field55, field56, field57, field58, fieldsec50, fieldsec51, fieldsec52, fieldsec53, fieldsec54, fieldsec55, fieldsec56, fieldsec57, fieldsec58 };
            List<int> parame7 = new List<int> { field60, field61, field62, field63, field64, field65, field66, field67, field68, fieldsec60, fieldsec61, fieldsec62, fieldsec63, fieldsec64, fieldsec65, fieldsec66, fieldsec67, fieldsec68 };
            List<int> parame8 = new List<int> { field70, field71, field72, field73, field74, field75, field76, field77, field78, fieldsec70, fieldsec71, fieldsec72, fieldsec73, fieldsec74, fieldsec75, fieldsec76, fieldsec77, fieldsec78 };
            List<int> parame9 = new List<int> { field80, field81, field82, field83, field84, field85, field86, field87, field88, fieldsec80, fieldsec81, fieldsec82, fieldsec83, fieldsec84, fieldsec85, fieldsec86, fieldsec87, fieldsec88 };
            List<int> parame10 = new List<int> { field90, field91, field92, field93, field94, field95, field96, field97, field98, fieldsec90, fieldsec91, fieldsec92, fieldsec93, fieldsec94, fieldsec95, fieldsec96, fieldsec97, fieldsec98 };
            List<int> parame11 = new List<int> { field100, field101, field102, field103, field104, field105, field106, field107, field108, fieldsec100, fieldsec101, fieldsec102, fieldsec103, fieldsec104, fieldsec105, fieldsec106, fieldsec107, fieldsec108 };
            List<int> parame12 = new List<int> { field110, field111, field112, field113, field114, field115, field116, field117, field118, fieldsec110, fieldsec111, fieldsec112, fieldsec113, fieldsec114, fieldsec115, fieldsec116, fieldsec117, fieldsec118 };
            List<int> parame13 = new List<int> { field120, field121, field122, field123, field124, field125, field126, field127, field128, fieldsec120, fieldsec121, fieldsec122, fieldsec123, fieldsec124, fieldsec125, fieldsec126, fieldsec127, fieldsec128 };
            List<int> parame14 = new List<int> { field130, field131, field132, field133, field134, field135, field136, field137, field138, fieldsec130, fieldsec131, fieldsec132, fieldsec133, fieldsec134, fieldsec135, fieldsec136, fieldsec137, fieldsec138 };
            List<int> parame15 = new List<int> { field140, field141, field142, field143, field144, field145, field146, field147, field148, fieldsec140, fieldsec141, fieldsec142, fieldsec143, fieldsec144, fieldsec145, fieldsec146, fieldsec147, fieldsec148 };
            List<int> parame16 = new List<int> { field150, field151, field152, field153, field154, field155, field156, field157, field158, fieldsec150, fieldsec151, fieldsec152, fieldsec153, fieldsec154, fieldsec155, fieldsec156, fieldsec157, fieldsec158 };
            List<int> parame17 = new List<int> { field160, field161, field162, field163, field164, field165, field166, field167, field168, fieldsec160, fieldsec161, fieldsec162, fieldsec163, fieldsec164, fieldsec165, fieldsec166, fieldsec167, fieldsec168 };
            List<int> parame18 = new List<int> { field170, field171, field172, field173, field174, field175, field176, field177, field178, fieldsec170, fieldsec171, fieldsec172, fieldsec173, fieldsec174, fieldsec175, fieldsec176, fieldsec177, fieldsec178 };
            List<int> parame19 = new List<int> { field180, field181, field182, field183, field184, field185, field186, field187, field188, fieldsec180, fieldsec181, fieldsec182, fieldsec183, fieldsec184, fieldsec185, fieldsec186, fieldsec187, fieldsec188 };
            List<int> parame20 = new List<int> { field190, field191, field192, field193, field194, field195, field196, field197, field198, fieldsec190, fieldsec191, fieldsec192, fieldsec193, fieldsec194, fieldsec195, fieldsec196, fieldsec197, fieldsec198 };
            List<int> parame21 = new List<int> { field200, field201, field202, field203, field204, field205, field206, field207, field208, fieldsec200, fieldsec201, fieldsec202, fieldsec203, fieldsec204, fieldsec205, fieldsec206, fieldsec207, fieldsec208 };
            List<int> parame22 = new List<int> { field210, field211, field212, field213, field214, field215, field216, field217, field218, fieldsec210, fieldsec211, fieldsec212, fieldsec213, fieldsec214, fieldsec215, fieldsec216, fieldsec217, fieldsec218 };
            List<int> parame23 = new List<int> { field220, field221, field222, field223, field224, field225, field226, field227, field228, fieldsec220, fieldsec221, fieldsec222, fieldsec223, fieldsec224, fieldsec225, fieldsec226, fieldsec227, fieldsec228 };
            List<int> parame24 = new List<int> { field230, field231, field232, field233, field234, field235, field236, field237, field238, fieldsec230, fieldsec231, fieldsec232, fieldsec233, fieldsec234, fieldsec235, fieldsec236, fieldsec237, fieldsec238 };
            List<int> parame25 = new List<int> { field240, field241, field242, field243, field244, field245, field246, field247, field248, fieldsec240, fieldsec241, fieldsec242, fieldsec243, fieldsec244, fieldsec245, fieldsec246, fieldsec247, fieldsec248 };
            List<int> parame26 = new List<int> { field250, field251, field252, field253, field254, field255, field256, field257, field258, fieldsec250, fieldsec251, fieldsec252, fieldsec253, fieldsec254, fieldsec255, fieldsec256, fieldsec257, fieldsec258 };
            List<int> parame27 = new List<int> { field260, field261, field262, field263, field264, field265, field266, field267, field268, fieldsec260, fieldsec261, fieldsec262, fieldsec263, fieldsec264, fieldsec265, fieldsec266, fieldsec267, fieldsec268 };
            List<int> parame28 = new List<int> { field270, field271, field272, field273, field274, field275, field276, field277, field278, fieldsec270, fieldsec271, fieldsec272, fieldsec273, fieldsec274, fieldsec275, fieldsec276, fieldsec277, fieldsec278 };
            List<int> parame29 = new List<int> { field280, field281, field282, field283, field284, field285, field286, field287, field288, fieldsec280, fieldsec281, fieldsec282, fieldsec283, fieldsec284, fieldsec285, fieldsec286, fieldsec287, fieldsec288 };
            List<int> parame30 = new List<int> { field290, field291, field292, field293, field294, field295, field296, field297, field298, fieldsec290, fieldsec291, fieldsec292, fieldsec293, fieldsec294, fieldsec295, fieldsec296, fieldsec297, fieldsec298 };
            List<int> parame31 = new List<int> { field300, field301, field302, field303, field304, field305, field306, field307, field308, fieldsec300, fieldsec301, fieldsec302, fieldsec303, fieldsec304, fieldsec305, fieldsec306, fieldsec307, fieldsec308 };
            List<int> parame32 = new List<int> { field310, field311, field312, field313, field314, field315, field316, field317, field318, fieldsec310, fieldsec311, fieldsec312, fieldsec313, fieldsec314, fieldsec315, fieldsec316, fieldsec317, fieldsec318 };
            parame.Add(parame2); parame.Add(parame3); parame.Add(parame4); parame.Add(parame5); parame.Add(parame6); parame.Add(parame7); parame.Add(parame8); parame.Add(parame9);
            parame.Add(parame10); parame.Add(parame11); parame.Add(parame12); parame.Add(parame13); parame.Add(parame14); parame.Add(parame15); parame.Add(parame16); parame.Add(parame17); parame.Add(parame18); parame.Add(parame19);
            parame.Add(parame20); parame.Add(parame21); parame.Add(parame22); parame.Add(parame23); parame.Add(parame24); parame.Add(parame25); parame.Add(parame26); parame.Add(parame27); parame.Add(parame28); parame.Add(parame29);
            parame.Add(parame30); parame.Add(parame31); parame.Add(parame32);

            int daysinmonth = DateTime.DaysInMonth(dataPlan_dateSelected.Year, dataPlan_dateSelected.Month);
            for (int i = 1; i <= daysinmonth; i++)
            {
                DateTime dateToEdit = new DateTime(dataPlan_dateSelected.Year, dataPlan_dateSelected.Month, i);
                for (int j = 0; j <= 17; j++)
                {
                    List<TDataplant> dataToEdit = _ctx2.TDataplants.Where(s => DateTime.Compare(s.FDate, dateToEdit) == 0).ToList();
                    string sql = "EXEC dbo.Edit_DataPlan @Value,@DataId";
                    List<SqlParameter> parms = new List<SqlParameter>
                    {new SqlParameter { ParameterName = "@Value", Value =parame[i-1][j]},
                     new SqlParameter { ParameterName = "@DataId", Value =dataToEdit[j].FId}};
                    int rowsAffected;
                    rowsAffected = _ctx2.Database.ExecuteSqlRaw(sql, parms.ToArray());
                }
            }






            return RedirectToAction("InputDataPlan");
        }


        public IActionResult SwitchLangToEN()
        {
            langSelected = "EN";
            ViewBag.langSelected = langSelected;
            return Redirect(ControllerContext.HttpContext.Request.Headers["Referer"].ToString());
        }

        public IActionResult SwitchLangToDE()
        {
            langSelected = "DE";
            ViewBag.langSelected = langSelected;
            return Redirect(ControllerContext.HttpContext.Request.Headers["Referer"].ToString());
        }

        public IActionResult SwitchLangToFR()
        {
            langSelected = "FR";
            ViewBag.langSelected = langSelected;
            return Redirect(ControllerContext.HttpContext.Request.Headers["Referer"].ToString());
        }

        public IActionResult EditReport40(int DaySelected,int MonthSelected,int YearSelected)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;


            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                DateTime dateSelected2 = new DateTime(YearSelected, MonthSelected, DaySelected, 0, 0, 0);
                DateSelected = dateSelected2;

                List<TShift> shiftToEdit = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall 4.0")) && (s.FDate.Day==dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year)).ToList();

                List<TDatarecord> dataToEdit1A = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1C = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1NONE = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2A = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2C = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2NONE = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3A = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3C = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3NONE = new List<TDatarecord>();

                foreach (var item in shiftToEdit)
                {
                    foreach (var tar in _ctx2.TDatarecords.ToList())
                    {
                        if (tar.FShiftid == item.FShiftid)
                        {
                            if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("A")))
                            {
                                dataToEdit1A.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("C")))
                            {
                                dataToEdit1C.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("None")))
                            {
                                dataToEdit1NONE.Add(tar);
                            }


                            if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("A")))
                            {
                                dataToEdit2A.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("C")))
                            {
                                dataToEdit2C.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("None")))
                            {
                                dataToEdit2NONE.Add(tar);
                            }

                            if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("A")))
                            {
                                dataToEdit3A.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("C")))
                            {
                                dataToEdit3C.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("None")))
                            {
                                dataToEdit3NONE.Add(tar);
                            }
                        }
                    }
                }

                foreach (var elt in dataToEdit1A)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s =>(s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year) && (s.FTypeid == 33)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit1C)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year) && (s.FTypeid == 42)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit2A)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year) && (s.FTypeid == 34)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit2C)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year) && (s.FTypeid == 44)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit3A)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year) && (s.FTypeid == 35)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit3C)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year) && (s.FTypeid == 45)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }



                List<int> OrderOfTypeId_Section = new List<int> { 1, 2, 3, 4, 13, 5, 12, 6, 9, 7, 10, 11, 23 };
                List<int> OrderOfTypeId_Prod40 = new List<int> { 16, 17, 24, 20, 18, 21, 22, 19, 29, 15 };

                List<TDatarecord> dataToEdit1AOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1COrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1NONEOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2AOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2COrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2NONEOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3AOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3COrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3NONEOrdered = new List<TDatarecord>();

                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit1A)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit1AOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit1C)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit1COrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit2A)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit2AOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit2C)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit2COrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit3A)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit3AOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit3C)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit3COrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Prod40)
                {
                    foreach (var item in dataToEdit1NONE)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit1NONEOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Prod40)
                {
                    foreach (var item in dataToEdit2NONE)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit2NONEOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Prod40)
                {
                    foreach (var item in dataToEdit3NONE)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit3NONEOrdered.Add(item);
                        }
                    }
                }





                ViewBag.dataToEdit1A = dataToEdit1AOrdered;
                ViewBag.dataToEdit2A = dataToEdit2AOrdered;
                ViewBag.dataToEdit3A = dataToEdit3AOrdered;
                ViewBag.dataToEdit1C = dataToEdit1COrdered;
                ViewBag.dataToEdit2C = dataToEdit2COrdered;
                ViewBag.dataToEdit3C = dataToEdit3COrdered;
                ViewBag.dataToEdit1NONE = dataToEdit1NONEOrdered;
                ViewBag.dataToEdit2None = dataToEdit2NONEOrdered;
                ViewBag.dataToEdit3NONE = dataToEdit3NONEOrdered;




                ViewBag.ErrorExist = _ctx2.TTranslations.Where(s => (s.FLabelid == 5) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 6) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 7) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ProdAMG = _ctx2.TTranslations.Where(s => (s.FLabelid == 8) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Shift = _ctx2.TTranslations.Where(s => (s.FLabelid == 9) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SOLLProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 10) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ISTProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 11) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TL = _ctx2.TTranslations.Where(s => (s.FLabelid == 12) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.PD = _ctx2.TTranslations.Where(s => (s.FLabelid == 13) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.NewMA = _ctx2.TTranslations.Where(s => (s.FLabelid == 14) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SpeMeas = _ctx2.TTranslations.Where(s => (s.FLabelid == 15) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.AS = _ctx2.TTranslations.Where(s => (s.FLabelid == 16) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TrainerZNA = _ctx2.TTranslations.Where(s => (s.FLabelid == 17) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Vac = _ctx2.TTranslations.Where(s => (s.FLabelid == 18) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.VacIND = _ctx2.TTranslations.Where(s => (s.FLabelid == 19) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sick = _ctx2.TTranslations.Where(s => (s.FLabelid == 20) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TempW = _ctx2.TTranslations.Where(s => (s.FLabelid == 21) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SumPerSec = _ctx2.TTranslations.Where(s => (s.FLabelid == 22) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sec = _ctx2.TTranslations.Where(s => (s.FLabelid == 23) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod90 = _ctx2.TTranslations.Where(s => (s.FLabelid == 24) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZNASecA = _ctx2.TTranslations.Where(s => (s.FLabelid == 25) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZNASecC = _ctx2.TTranslations.Where(s => (s.FLabelid == 26) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TotSets = _ctx2.TTranslations.Where(s => (s.FLabelid == 27) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.BuffLev = _ctx2.TTranslations.Where(s => (s.FLabelid == 28) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TeamDiss = _ctx2.TTranslations.Where(s => (s.FLabelid == 29) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ShiftDurBreak = _ctx2.TTranslations.Where(s => (s.FLabelid == 30) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeTargActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 31) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZQB = _ctx2.TTranslations.Where(s => (s.FLabelid == 32) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SKD = _ctx2.TTranslations.Where(s => (s.FLabelid == 33) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Valmet = _ctx2.TTranslations.Where(s => (s.FLabelid == 34) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sindel = _ctx2.TTranslations.Where(s => (s.FLabelid == 35) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Rastatt = _ctx2.TTranslations.Where(s => (s.FLabelid == 36) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.RA41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 37) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Kecs = _ctx2.TTranslations.Where(s => (s.FLabelid == 38) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.RA40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 39) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeTarg = _ctx2.TTranslations.Where(s => (s.FLabelid == 40) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 41) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ShiftDur = _ctx2.TTranslations.Where(s => (s.FLabelid == 42) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Break = _ctx2.TTranslations.Where(s => (s.FLabelid == 43) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SOLLGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 44) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ISTGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 45) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SumProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 46) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Edit = _ctx2.TTranslations.Where(s => (s.FLabelid == 75) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.submit = _ctx2.TTranslations.Where(s => (s.FLabelid == 2) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


                return View();
            }
        }

        public IActionResult EditReport41(int DaySelected, int MonthSelected, int YearSelected)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;


            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                DateTime dateSelected2 = new DateTime(YearSelected, MonthSelected, DaySelected, 0, 0, 0);
                DateSelected = dateSelected2;

                List<TShift> shiftToEdit = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall 4.1")) && (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year)).ToList();

                List<TDatarecord> dataToEdit1A = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1C = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1NONE = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2A = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2C = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2NONE = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3A = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3C = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3NONE = new List<TDatarecord>();


                foreach (var item in shiftToEdit)
                {
                    foreach (var tar in _ctx2.TDatarecords.ToList())
                    {
                        if (tar.FShiftid == item.FShiftid)
                        {
                            if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("A")))
                            {
                                dataToEdit1A.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("C")))
                            {
                                dataToEdit1C.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("None")))
                            {
                                dataToEdit1NONE.Add(tar);
                            }


                            if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("A")))
                            {
                                dataToEdit2A.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("C")))
                            {
                                dataToEdit2C.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("None")))
                            {
                                dataToEdit2NONE.Add(tar);
                            }


                            if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("A")))
                            {
                                dataToEdit3A.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("C")))
                            {
                                dataToEdit3C.Add(tar);
                            }
                            if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("None")))
                            {
                                dataToEdit3NONE.Add(tar);
                            }

                        }
                    }
                }

                foreach (var elt in dataToEdit1A)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 36)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit1C)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 46)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit2A)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 37)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit2C)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 47)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit3A)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 38)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }
                foreach (var elt in dataToEdit3C)
                {
                    if (elt.FRecordtypeid == 1)
                    {
                        TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 48)).FirstOrDefault();
                        if (temp != null)
                        {
                            elt.FRecordvalue = temp.FValue;
                        }
                    }
                }


                List<int> OrderOfTypeId_Section = new List<int> { 1, 2, 3, 4, 13, 5, 12, 6, 9, 7, 10, 11, 23 };
                List<int> OrderOfTypeId_Prod41 = new List<int> { 16, 17, 31, 26, 30, 32, 24, 20, 18, 21, 22, 19, 29, 15 };

                List<TDatarecord> dataToEdit1AOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1COrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit1NONEOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2AOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2COrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit2NONEOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3AOrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3COrdered = new List<TDatarecord>();
                List<TDatarecord> dataToEdit3NONEOrdered = new List<TDatarecord>();

                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit1A)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit1AOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit1C)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit1COrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit2A)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit2AOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit2C)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit2COrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit3A)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit3AOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Section)
                {
                    foreach (var item in dataToEdit3C)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit3COrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Prod41)
                {
                    foreach (var item in dataToEdit1NONE)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit1NONEOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Prod41)
                {
                    foreach (var item in dataToEdit2NONE)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit2NONEOrdered.Add(item);
                        }
                    }
                }
                foreach (var elt in OrderOfTypeId_Prod41)
                {
                    foreach (var item in dataToEdit3NONE)
                    {
                        if (item.FTypeid == elt)
                        {
                            dataToEdit3NONEOrdered.Add(item);
                        }
                    }
                }

                ViewBag.dataToEdit1A = dataToEdit1AOrdered;
                ViewBag.dataToEdit2A = dataToEdit2AOrdered;
                ViewBag.dataToEdit3A = dataToEdit3AOrdered;
                ViewBag.dataToEdit1C = dataToEdit1COrdered;
                ViewBag.dataToEdit2C = dataToEdit2COrdered;
                ViewBag.dataToEdit3C = dataToEdit3COrdered;
                ViewBag.dataToEdit1NONE = dataToEdit1NONEOrdered;
                ViewBag.dataToEdit2None = dataToEdit2NONEOrdered;
                ViewBag.dataToEdit3NONE = dataToEdit3NONEOrdered;


                ViewBag.ErrorExist = _ctx2.TTranslations.Where(s => (s.FLabelid == 5) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 6) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 7) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ProdAMG = _ctx2.TTranslations.Where(s => (s.FLabelid == 8) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Shift = _ctx2.TTranslations.Where(s => (s.FLabelid == 9) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SOLLProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 10) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ISTProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 11) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TL = _ctx2.TTranslations.Where(s => (s.FLabelid == 12) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.PD = _ctx2.TTranslations.Where(s => (s.FLabelid == 13) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.NewMA = _ctx2.TTranslations.Where(s => (s.FLabelid == 14) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SpeMeas = _ctx2.TTranslations.Where(s => (s.FLabelid == 15) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.AS = _ctx2.TTranslations.Where(s => (s.FLabelid == 16) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TrainerZNA = _ctx2.TTranslations.Where(s => (s.FLabelid == 17) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Vac = _ctx2.TTranslations.Where(s => (s.FLabelid == 18) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.VacIND = _ctx2.TTranslations.Where(s => (s.FLabelid == 19) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sick = _ctx2.TTranslations.Where(s => (s.FLabelid == 20) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TempW = _ctx2.TTranslations.Where(s => (s.FLabelid == 21) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SumPerSec = _ctx2.TTranslations.Where(s => (s.FLabelid == 22) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sec = _ctx2.TTranslations.Where(s => (s.FLabelid == 23) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod90 = _ctx2.TTranslations.Where(s => (s.FLabelid == 24) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZNASecA = _ctx2.TTranslations.Where(s => (s.FLabelid == 25) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZNASecC = _ctx2.TTranslations.Where(s => (s.FLabelid == 26) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TotSets = _ctx2.TTranslations.Where(s => (s.FLabelid == 27) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.BuffLev = _ctx2.TTranslations.Where(s => (s.FLabelid == 28) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TeamDiss = _ctx2.TTranslations.Where(s => (s.FLabelid == 29) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ShiftDurBreak = _ctx2.TTranslations.Where(s => (s.FLabelid == 30) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeTargActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 31) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZQB = _ctx2.TTranslations.Where(s => (s.FLabelid == 32) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SKD = _ctx2.TTranslations.Where(s => (s.FLabelid == 33) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Valmet = _ctx2.TTranslations.Where(s => (s.FLabelid == 34) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sindel = _ctx2.TTranslations.Where(s => (s.FLabelid == 35) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Rastatt = _ctx2.TTranslations.Where(s => (s.FLabelid == 36) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.RA41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 37) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Kecs = _ctx2.TTranslations.Where(s => (s.FLabelid == 38) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.RA40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 39) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeTarg = _ctx2.TTranslations.Where(s => (s.FLabelid == 40) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 41) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ShiftDur = _ctx2.TTranslations.Where(s => (s.FLabelid == 42) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Break = _ctx2.TTranslations.Where(s => (s.FLabelid == 43) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SOLLGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 44) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ISTGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 45) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SumProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 46) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Edit = _ctx2.TTranslations.Where(s => (s.FLabelid == 75) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.submit = _ctx2.TTranslations.Where(s => (s.FLabelid == 2) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();




                return View();
            }
        }


        public IActionResult EditReportAMG(int DaySelected, int MonthSelected, int YearSelected)
        {
            ViewBag.Navbar1 = _ctx2.TTranslations.Where(s => (s.FLabelid == 51) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.Navbar2 = _ctx2.TTranslations.Where(s => (s.FLabelid == 52) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.login = _ctx2.TTranslations.Where(s => (s.FLabelid == 49) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.coas = _ctx2.TTranslations.Where(s => (s.FLabelid == 53) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
            ViewBag.logout = _ctx2.TTranslations.Where(s => (s.FLabelid == 54) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();


            ViewBag.langSelected = langSelected;
            ViewBag.CurrentUserId = CurrentUser.FUserId;
            ViewBag.CurrentUserRole = CurrentUser.FRole;


            if (CurrentUser.FRole == null)
            {
                return RedirectToAction("Connexion");
            }
            if (CurrentUser.FRole != "editor")
            {
                return RedirectToAction("AccessDenied");
            }
            else
            {
                DateTime dateSelected2 = new DateTime(YearSelected, MonthSelected, DaySelected, 0, 0, 0);
                DateSelected = dateSelected2;

            List<TShift> shiftToEdit = _ctx2.TShifts.Where(s => (s.FHall.Equals("Hall AMG")) && (s.FDate.Day == dateSelected2.Day) && (s.FDate.Month == dateSelected2.Month) && (s.FDate.Year == dateSelected2.Year)).ToList();
            List<TDatarecord> dataToEdit1NONE = new List<TDatarecord>();
            List<TDatarecord> dataToEdit2NONE = new List<TDatarecord>();
            List<TDatarecord> dataToEdit3NONE = new List<TDatarecord>();


            foreach (var item in shiftToEdit)
            {
                foreach (var tar in _ctx2.TDatarecords.ToList())
                {
                    if (tar.FShiftid == item.FShiftid)
                    {
                        if ((item.FShift.Equals("Layer 1")) && (item.FSection.Equals("None")))
                        {
                            dataToEdit1NONE.Add(tar);
                        }
                        if ((item.FShift.Equals("Layer 2")) && (item.FSection.Equals("None")))
                        {
                            dataToEdit2NONE.Add(tar);
                        }
                        if ((item.FShift.Equals("Layer 3")) && (item.FSection.Equals("None")))
                        {
                            dataToEdit3NONE.Add(tar);
                        }

                    }
                }
            }

            foreach (var elt in dataToEdit1NONE)
            {
                if (elt.FRecordtypeid == 1)
                {
                    TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 49)).FirstOrDefault();
                    if (temp != null)
                    {
                        elt.FRecordvalue = temp.FValue;
                    }
                }
            }
            foreach (var elt in dataToEdit2NONE)
            {
                if (elt.FRecordtypeid == 1)
                {
                    TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 50)).FirstOrDefault();
                    if (temp != null)
                    {
                        elt.FRecordvalue = temp.FValue;
                    }
                }
            }
            foreach (var elt in dataToEdit3NONE)
            {
                if (elt.FRecordtypeid == 1)
                {
                    TDataplant temp = _ctx2.TDataplants.Where(s => (DateTime.Compare(s.FDate, dateSelected2) == 0) && (s.FTypeid == 51)).FirstOrDefault();
                    if (temp != null)
                    {
                        elt.FRecordvalue = temp.FValue;
                    }
                }
            }

            List<int> OrderOfTypeId_AMG = new List<int> { 1, 2, 3, 4, 13, 5, 12, 6, 9, 7, 10, 11, 23, 16, 24, 25, 26, 27, 28, 19, 18, 21, 22, 29, 15 };

            List<TDatarecord> dataToEdit1NONEOrdered = new List<TDatarecord>();
            List<TDatarecord> dataToEdit2NONEOrdered = new List<TDatarecord>();
            List<TDatarecord> dataToEdit3NONEOrdered = new List<TDatarecord>();

            foreach (var elt in OrderOfTypeId_AMG)
            {
                foreach (var item in dataToEdit1NONE)
                {
                    if (item.FTypeid == elt)
                    {
                        dataToEdit1NONEOrdered.Add(item);
                    }
                }
            }
            foreach (var elt in OrderOfTypeId_AMG)
            {
                foreach (var item in dataToEdit2NONE)
                {
                    if (item.FTypeid == elt)
                    {
                        dataToEdit2NONEOrdered.Add(item);
                    }
                }
            }
            foreach (var elt in OrderOfTypeId_AMG)
            {
                foreach (var item in dataToEdit3NONE)
                {
                    if (item.FTypeid == elt)
                    {
                        dataToEdit3NONEOrdered.Add(item);
                    }
                }
            }

            ViewBag.dataToEdit1NONE = dataToEdit1NONEOrdered;
            ViewBag.dataToEdit2None = dataToEdit2NONEOrdered;
            ViewBag.dataToEdit3NONE = dataToEdit3NONEOrdered;



                ViewBag.ErrorExist = _ctx2.TTranslations.Where(s => (s.FLabelid == 5) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 6) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 7) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ProdAMG = _ctx2.TTranslations.Where(s => (s.FLabelid == 8) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Shift = _ctx2.TTranslations.Where(s => (s.FLabelid == 9) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SOLLProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 10) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ISTProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 11) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TL = _ctx2.TTranslations.Where(s => (s.FLabelid == 12) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.PD = _ctx2.TTranslations.Where(s => (s.FLabelid == 13) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.NewMA = _ctx2.TTranslations.Where(s => (s.FLabelid == 14) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SpeMeas = _ctx2.TTranslations.Where(s => (s.FLabelid == 15) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.AS = _ctx2.TTranslations.Where(s => (s.FLabelid == 16) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TrainerZNA = _ctx2.TTranslations.Where(s => (s.FLabelid == 17) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Vac = _ctx2.TTranslations.Where(s => (s.FLabelid == 18) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.VacIND = _ctx2.TTranslations.Where(s => (s.FLabelid == 19) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sick = _ctx2.TTranslations.Where(s => (s.FLabelid == 20) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TempW = _ctx2.TTranslations.Where(s => (s.FLabelid == 21) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SumPerSec = _ctx2.TTranslations.Where(s => (s.FLabelid == 22) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sec = _ctx2.TTranslations.Where(s => (s.FLabelid == 23) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Prod90 = _ctx2.TTranslations.Where(s => (s.FLabelid == 24) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZNASecA = _ctx2.TTranslations.Where(s => (s.FLabelid == 25) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZNASecC = _ctx2.TTranslations.Where(s => (s.FLabelid == 26) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TotSets = _ctx2.TTranslations.Where(s => (s.FLabelid == 27) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.BuffLev = _ctx2.TTranslations.Where(s => (s.FLabelid == 28) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.TeamDiss = _ctx2.TTranslations.Where(s => (s.FLabelid == 29) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ShiftDurBreak = _ctx2.TTranslations.Where(s => (s.FLabelid == 30) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeTargActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 31) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ZQB = _ctx2.TTranslations.Where(s => (s.FLabelid == 32) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SKD = _ctx2.TTranslations.Where(s => (s.FLabelid == 33) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Valmet = _ctx2.TTranslations.Where(s => (s.FLabelid == 34) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Sindel = _ctx2.TTranslations.Where(s => (s.FLabelid == 35) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Rastatt = _ctx2.TTranslations.Where(s => (s.FLabelid == 36) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.RA41 = _ctx2.TTranslations.Where(s => (s.FLabelid == 37) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Kecs = _ctx2.TTranslations.Where(s => (s.FLabelid == 38) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.RA40 = _ctx2.TTranslations.Where(s => (s.FLabelid == 39) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeTarg = _ctx2.TTranslations.Where(s => (s.FLabelid == 40) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.CycleTimeActual = _ctx2.TTranslations.Where(s => (s.FLabelid == 41) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ShiftDur = _ctx2.TTranslations.Where(s => (s.FLabelid == 42) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Break = _ctx2.TTranslations.Where(s => (s.FLabelid == 43) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SOLLGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 44) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.ISTGes = _ctx2.TTranslations.Where(s => (s.FLabelid == 45) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.SumProd = _ctx2.TTranslations.Where(s => (s.FLabelid == 46) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.Edit = _ctx2.TTranslations.Where(s => (s.FLabelid == 75) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();
                ViewBag.submit = _ctx2.TTranslations.Where(s => (s.FLabelid == 2) && (s.FLanguage.Equals(langSelected))).FirstOrDefault();




                return View();
        }
        }


    }
}





