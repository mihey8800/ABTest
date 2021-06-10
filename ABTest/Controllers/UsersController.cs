using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Models.Repositories;
using Models.Shared;
using Newtonsoft.Json;
using Services;
using Services.UserServices;

namespace ABTest.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly IService<CalculateRollingRetentionXdayContext, IEnumerable<RollingRetentionXDay>> _rollingRetentionXDay;
        private readonly IService<CalculateUserLifetimeContext, IEnumerable<UserLifetime>> _rollingRetention7Day;
        private readonly IUserRepository _usersRepository;
        private readonly IConfiguration _configuration;

        public UsersController(
            IService<CalculateRollingRetentionXdayContext, IEnumerable<RollingRetentionXDay>> rollingRetentionXDay,
            IService<CalculateUserLifetimeContext, IEnumerable<UserLifetime>> rollingRetention7Day,
            IUserRepository userRepository, IConfiguration configuration)
        {
            _rollingRetentionXDay = rollingRetentionXDay;
            _rollingRetention7Day = rollingRetention7Day;
            _usersRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns>List of Users</returns>
        /// <response code="200">Successful API response</response>
        [Route("GetAllUsers")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserParameters userParameters,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _usersRepository.GetUserList(cancellationToken, userParameters);

                var metadata = new
                {
                    users.TotalCount,
                    users.PageSize,
                    users.CurrentPage,
                    users.TotalPages,
                    users.HasNext,
                    users.HasPrevious
                };
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
                return Json(users);
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        /// <summary>
        /// Get calculated User Rolling 7 Days Retentions
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of Rolling X Days Retentions</returns>
        [Route("GetUsersUsersLiveTime")]
        [HttpGet]
        public async Task<IActionResult> GetUsersUsersLiveTime(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _usersRepository.GetUserList(cancellationToken);
                if (users == null)
                {
                    return BadRequest(null);
                }

                var usersList = users.ToList();
                var result = _rollingRetention7Day.Execute(new CalculateUserLifetimeContext
                {
                    Users = usersList,
                    CancellationToken = cancellationToken
                });

                return Json(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }


        /// <summary>
        /// Get calculated User Rolling 7 Days Retentions
        /// </summary>
        /// <param name="daysCount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of Rolling X Days Retentions</returns>
        [Route("GetUsersRollingRetentionsXDay/{daysCount}")]
        [HttpGet]
        public async Task<IActionResult> GetUsersRollingRetentionsXDay(int daysCount, CancellationToken cancellationToken = default)
        {
            try
            {
                if (daysCount <= 0)
                {
                    return Json(new List<RollingRetentionXDay>());
                }
                var users = await _usersRepository.GetUserList(cancellationToken);
                if (users == null)
                {
                    return BadRequest(null);
                }

                var usersList = users.ToList();
                var result = await _rollingRetentionXDay.ExecuteAsync(new CalculateRollingRetentionXdayContext
                {
                    DaysCount = daysCount,
                    Users = usersList,
                    CancellationToken = cancellationToken
                });

                return Json(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }


        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="user">Form data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!TryValidateModel(user))
                {
                    return BadRequest();
                }
                var updatedUser = await _usersRepository.UpdateUser(user, cancellationToken);
                if (updatedUser == null)
                {
                    return BadRequest();
                }

                return Json(updatedUser);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }


        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="users">Form data</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("CreateOrUpdateUsers")]
        public async Task<IActionResult> CreateOrUpdateUsers([FromBody] List<User> users,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var usersToUpdate = users.Where(a => a.Id != 0).ToList();
                if (usersToUpdate.Count > 0)
                {
                    await _usersRepository.UpdateUsers(usersToUpdate, cancellationToken);
                }

                var usersToCreate = users.Where(a => a.Id == 0).ToList();
                if (usersToCreate.Count > 0)
                {
                    await _usersRepository.CreateUsers(usersToCreate, cancellationToken);
                }


                return Json(users);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }
    }
}