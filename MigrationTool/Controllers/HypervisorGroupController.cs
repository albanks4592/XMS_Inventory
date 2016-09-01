//------------------------------------------------------------------------------
// <copyright file="HypervisorGroupController.cs" company="Novartis">
//      Copyright (c) Novartis AG
// </copyright>
//------------------------------------------------------------------------------

namespace MigrationTool.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;
    using MigrationTool.Models;
    using MigrationTool.ViewModels;

    /// <summary>
    /// MVC Controller for the HypervisorGroup entity.
    /// </summary>
    [Authorize(Roles = MigrationTool.Roles.View + ", " + MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
    public class HypervisorGroupController : Controller
    {
        /// <summary>
        /// The database context that all class methods will use for data
        /// gathering.
        /// </summary>
        private MigrationToolEntities db = new MigrationToolEntities();

        /// <summary>
        /// Index action (GET).
        /// </summary>
        /// <returns>The index listing of the HypervisorGroup entity.</returns>
        public ActionResult Index()
        {
            if (this.db.HypervisorGroups.Count() > 50)
            {
                return this.View((IEnumerable<HypervisorGroupListIndexViewModel>)null);
            }
            else
            {
                return this.View(HypervisorGroupListIndexViewModel.SelectMany(this.db, null));
            }
        }

        /// <summary>
        /// AJAX action for filtering the index listing.
        /// </summary>
        /// <param name="filter">A string to use to filter the Name property of
        /// the HypervisorGroup returned.</param>
        /// <param name="tagFilter">A list of tags to apply as a filter.</param>
        /// <param name="noteInfo">A flag indicating that only records that have
        /// one or more info type notes linked should be included.</param>
        /// <param name="noteSuccess">A flag indicating that only records that
        /// have one or more success type notes linked should be
        /// included.</param>
        /// <param name="noteWarning">A flag indicating that only records that
        /// have one or more warning type notes linked should be
        /// included.</param>
        /// <param name="noteDanger">A flag indicating that only records that
        /// have one or more danger type notes linked should be
        /// included.</param>
        /// <param name="noActiveRecordsLink">A flag indicating that only 
        /// records that are active will be displayed.</param>
        /// <returns>The filtered index listing.</returns>
        public PartialViewResult FilterIndex(string filter, string[] tagFilter, bool noteInfo, bool noteSuccess, bool noteWarning, bool noteDanger, bool noActiveRecordsLink = false)
        {
            var results = this.db.HypervisorGroups
                .AsQueryable()
                .Include("TagsMetas")
                .Include("TagsMetas.Tag")
                .Include("Notes");

            if (!string.IsNullOrEmpty(filter))
            {
                results = results.Where(x => x.Name.Contains(filter));
            }

            if (tagFilter != null)
            {
                foreach (string tag in tagFilter)
                {
                    string trimTag = tag.Trim().ToLower();
                    results = results.Where(x => x.TagsMetas.Any(m => m.Tag.Name == trimTag));
                }
            }

            if (noteInfo)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 0));
            }

            if (noteSuccess)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 1));
            }

            if (noteWarning)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 2));
            }

            if (noteDanger)
            {
                results = results.Where(x => x.Notes.Any(n => n.NoteType == 3));
            }

            if (noActiveRecordsLink)
            {
                results = results.Where(a => a.Inactive == false);
            }

            var viewModel = results
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Name)
                .AsEnumerable()
                .Select(x => new HypervisorGroupListIndexViewModel(x))
                .ToList();

            return this.PartialView("_ListIndex", viewModel);
        }

        /// <summary>
        /// Details action (GET).
        /// </summary>
        /// <param name="id">The ID of the HypervisorGroup to view.</param>
        /// <returns>The details view for the specified 
        /// HypervisorGroup.</returns>
        public ActionResult Details(Guid id)
        {
            HypervisorGroupDetailsViewModel item = HypervisorGroupDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            ViewBag.HypervisorWorkloadProfile_Id_Select = new SelectList(db.HypervisorWorkloadProfiles.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.CapacityRules = db.CapacityRules
                .Where(x => x.RuleType != 3)
                .AsEnumerable()
                .Select(x => new CapacityRuleReferenceViewModel(x))
                .ToList();

            return this.View(item);
        }

        /// <summary>
        /// Create action (GET).
        /// </summary>
        /// <returns>A blank create form for adding a new 
        /// HypervisorGroup.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create()
        {
            ViewBag.HypervisorGroupType_Id_Select = new SelectList(this.db.HypervisorGroupTypes.OrderBy(x => x.Name), "Id", "Name");
            return this.View();
        }

        /// <summary>
        /// Create action (POST).
        /// </summary>
        /// <param name="hypervisorgroup">The HypervisorGroup to
        /// create.</param>
        /// <returns>If creation is successful, redirects to the details page
        /// for the newly created HypervisorGroup. If there are any errors the
        /// create form is shown again with appropriate validation
        /// messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Create(HypervisorGroup hypervisorgroup)
        {
            if (ModelState.IsValid)
            {
                hypervisorgroup.Id = Guid.NewGuid();
                hypervisorgroup.CreatedDate = DateTime.Now;
                hypervisorgroup.LastUpdated = hypervisorgroup.CreatedDate;

                if (hypervisorgroup.HighAvailabilityCalculationType != 1)
                {
                    hypervisorgroup.HighAvailabilityReservedHypervisors = 0;
                }

                if (hypervisorgroup.HighAvailabilityCalculationType != 2)
                {
                    hypervisorgroup.HighAvailabilityReservedPercentage = 0;
                }

                this.db.HypervisorGroups.AddObject(hypervisorgroup);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = hypervisorgroup.Id });
            }

            ViewBag.HypervisorGroupType_Id_Select = new SelectList(this.db.HypervisorGroupTypes.OrderBy(x => x.Name), "Id", "Name", hypervisorgroup.HypervisorGroupType_Id);
            return this.View(hypervisorgroup);
        }

        /// <summary>
        /// Edit action (GET).
        /// </summary>
        /// <param name="id">The ID of the HypervisorGroup to edit.</param>
        /// <returns>The edit form for the specified HypervisorGroup.</returns>
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(Guid id)
        {
            HypervisorGroup hypervisorgroup = this.db.HypervisorGroups.Single(h => h.Id == id);
            if (hypervisorgroup == null)
            {
                return this.HttpNotFound();
            }

            ViewBag.HypervisorGroupType_Id_Select = new SelectList(this.db.HypervisorGroupTypes.OrderBy(x => x.Name), "Id", "Name", hypervisorgroup.HypervisorGroupType_Id);
            return this.View(hypervisorgroup);
        }

        /// <summary>
        /// Edit action (POST).
        /// </summary>
        /// <param name="hypervisorgroup">The modified HypervisorGroup to 
        /// record to the database.</param>
        /// <returns>If the update is successful, redirects to the details page
        /// for the HypervisorGroup. If there are any errors the edit form is
        /// shown again with appropriate validation messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult Edit(HypervisorGroup hypervisorgroup)
        {
            if (ModelState.IsValid)
            {
                hypervisorgroup.LastUpdated = DateTime.Now;

                if (hypervisorgroup.HighAvailabilityCalculationType != 1)
                {
                    hypervisorgroup.HighAvailabilityReservedHypervisors = 0;
                }

                if (hypervisorgroup.HighAvailabilityCalculationType != 2)
                {
                    hypervisorgroup.HighAvailabilityReservedPercentage = 0;
                }

                if (hypervisorgroup.Inactive)
                {
                    if (hypervisorgroup.InactiveDate == null)
                    {
                        hypervisorgroup.InactiveDate = hypervisorgroup.LastUpdated;
                    }
                }
                else
                {
                    if (hypervisorgroup.InactiveDate != null)
                    {
                        hypervisorgroup.InactiveDate = null;
                    }
                }

                this.db.HypervisorGroups.Attach(hypervisorgroup);
                this.db.ObjectStateManager.ChangeObjectState(hypervisorgroup, EntityState.Modified);
                this.db.SaveChanges();
                return this.RedirectToAction("Details", new { id = hypervisorgroup.Id });
            }

            ViewBag.HypervisorGroupType_Id_Select = new SelectList(this.db.HypervisorGroupTypes.OrderBy(x => x.Name), "Id", "Name", hypervisorgroup.HypervisorGroupType_Id);
            return this.View(hypervisorgroup);
        }

        /// <summary>
        /// Sets the Work load profile of a Hypervisor.
        /// </summary>
        /// <param name="hypervisorGroup_Id">The ID of the Hypervisor who's
        /// profile is to be set.</param>
        /// <param name="hypervisorWorkloadProfile_Id">The ID of the Work load
        /// profile is to be set.</param>
        /// <returns>If the update is successful, redirects to the details page
        /// for the HypervisorGroup. If there are any errors the edit form is
        /// shown again with appropriate validation messages.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
        public ActionResult SetHypervisorWorkloadProfile(Guid hypervisorGroup_Id, Guid? hypervisorWorkloadProfile_Id)
        {
            if (hypervisorWorkloadProfile_Id.HasValue && hypervisorWorkloadProfile_Id != Guid.Empty)
            {
                var hypervisors = this.db.Hypervisors.Where(x => x.HypervisorGroup_Id == hypervisorGroup_Id);
                foreach (Hypervisor h in hypervisors)
                {
                    h.HypervisorWorkloadProfile_Id = hypervisorWorkloadProfile_Id;
                }

                this.db.SaveChanges();
            }

            return this.RedirectToAction("Details", new { id = hypervisorGroup_Id });
        }

        /// <summary>
        /// Delete action (GET).
        /// </summary>
        /// <param name="id">The ID of the HypervisorGroup to
        /// delete.</param>
        /// <returns>The delete confirmation page for the specified
        /// HypervisorGroup.</returns>
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult Delete(Guid id)
        {
            HypervisorGroupDetailsViewModel item = HypervisorGroupDetailsViewModel.SelectSingle(this.db, x => x.Id == id);

            if (item == null)
            {
                return this.HttpNotFound();
            }

            return this.View(item);
        }

        /// <summary>
        /// Delete action (POST).
        /// </summary>
        /// <param name="id">The ID of the HypervisorGroup to
        /// delete.</param>
        /// <returns>Redirects to the index action.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = MigrationTool.Roles.Admin)]
        public ActionResult DeleteConfirmed(Guid id)
        {
            HypervisorGroup hypervisorgroup = this.db.HypervisorGroups.Single(h => h.Id == id);
            this.db.HypervisorGroups.DeleteObject(hypervisorgroup);
            this.db.SaveChanges();
            return this.RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the list of Virtual Machines associated with the 
        /// HypervisorGroup.
        /// </summary>
        /// <param name="id">The Id of the HypervisorGroup that we are 
        /// getting Virtual Machines for.</param>
        /// <returns>A list of Virtual Machines.</returns>
        public PartialViewResult GetVirtualMachineList(Guid id)
        {
            var virtualMachines = this.db.VirtualMachines.Where(x => x.Hypervisor.HypervisorGroup.Id == id).OrderBy(x => x.Name);
            return this.PartialView("../VirtualMachine/_List", virtualMachines.ToList());
        }

        /// <summary>
        /// Gets the list of Hypervisors associated with the 
        /// HypervisorGroup.
        /// </summary>
        /// <param name="id">The Id of the HypervisorGroup that we are 
        /// getting Hypervisors for.</param>
        /// <returns>A list of Hypervisors.</returns>
        public PartialViewResult GetHypervisorsList(Guid id)
        {
            var hypervisors = this.db.Hypervisors
                .Where(x => x.HypervisorGroup_Id == id)
                .OrderBy(x => x.Name)
                .AsEnumerable()
                .Select(x => new HypervisorListIndexViewModel(x))
                .ToList();
            return this.PartialView("../Hypervisor/_ListIndex", hypervisors);
        }

        /// <summary>
        /// Cleanly disposes of resources used by the controller.
        /// </summary>
        /// <param name="disposing">A flag indicating whether the call is coming
        /// from a Dispose method (true) or a finalize process (false).</param>
        protected override void Dispose(bool disposing)
        {
            this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}