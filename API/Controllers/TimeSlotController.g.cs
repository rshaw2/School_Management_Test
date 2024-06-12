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
    /// Controller responsible for managing timeslot related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting timeslot information.
    /// </remarks>
    [Route("api/timeslot")]
    [Authorize]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        /// <summary>
        /// Initializes a new instance of the TimeSlotController class with the specified context.
        /// </summary>
        /// <param name="itimeslotservice">The itimeslotservice to be used by the controller.</param>
        public TimeSlotController(ITimeSlotService itimeslotservice)
        {
            _timeSlotService = itimeslotservice;
        }

        /// <summary>Adds a new timeslot</summary>
        /// <param name="model">The timeslot data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("TimeSlot",Entitlements.Create)]
        public IActionResult Post([FromBody] TimeSlot model)
        {
            var id = _timeSlotService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of timeslots based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of timeslots</returns>
        [HttpGet]
        [UserAuthorize("TimeSlot",Entitlements.Read)]
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

            var result = _timeSlotService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <returns>The timeslot data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("TimeSlot",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _timeSlotService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("TimeSlot",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _timeSlotService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <param name="updatedEntity">The timeslot data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("TimeSlot",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] TimeSlot updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _timeSlotService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific timeslot by its primary key</summary>
        /// <param name="id">The primary key of the timeslot</param>
        /// <param name="updatedEntity">The timeslot data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("TimeSlot",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<TimeSlot> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _timeSlotService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}