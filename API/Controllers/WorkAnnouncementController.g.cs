using Microsoft.AspNetCore.Mvc;
using SchoolManagementTest.Models;
using SchoolManagementTest.Services;
using SchoolManagementTest.Entities;
using SchoolManagementTest.Filter;
using SchoolManagementTest.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace SchoolManagementTest.Controllers
{
    /// <summary>
    /// Controller responsible for managing workannouncement related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting workannouncement information.
    /// </remarks>
    [Route("api/workannouncement")]
    [Authorize]
    public class WorkAnnouncementController : ControllerBase
    {
        private readonly IWorkAnnouncementService _workAnnouncementService;

        /// <summary>
        /// Initializes a new instance of the WorkAnnouncementController class with the specified context.
        /// </summary>
        /// <param name="iworkannouncementservice">The iworkannouncementservice to be used by the controller.</param>
        public WorkAnnouncementController(IWorkAnnouncementService iworkannouncementservice)
        {
            _workAnnouncementService = iworkannouncementservice;
        }

        /// <summary>Adds a new workannouncement</summary>
        /// <param name="model">The workannouncement data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("WorkAnnouncement",Entitlements.Create)]
        public IActionResult Post([FromBody] WorkAnnouncement model)
        {
            var id = _workAnnouncementService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of workannouncements based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of workannouncements</returns>
        [HttpGet]
        [UserAuthorize("WorkAnnouncement",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _workAnnouncementService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <returns>The workannouncement data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("WorkAnnouncement",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _workAnnouncementService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("WorkAnnouncement",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _workAnnouncementService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <param name="updatedEntity">The workannouncement data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("WorkAnnouncement",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] WorkAnnouncement updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _workAnnouncementService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific workannouncement by its primary key</summary>
        /// <param name="id">The primary key of the workannouncement</param>
        /// <param name="updatedEntity">The workannouncement data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("WorkAnnouncement",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<WorkAnnouncement> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _workAnnouncementService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}