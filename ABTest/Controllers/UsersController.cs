using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IService<CalculateRollingRetentionContext, IEnumerable<UserRetention>> _rollingRetention;
        private readonly IUserRepository _usersRepository;
        private readonly IConfiguration _configuration;

        public UsersController(
            IService<CalculateRollingRetentionContext, IEnumerable<UserRetention>> rollingRetention,
            IUserRepository userRepository, IConfiguration configuration)
        {
            _rollingRetention = rollingRetention;
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
        /// Get calculated User Rolling X Days Retentions
        /// </summary>
        /// <param name="userParameters"></param>
        /// <param name="days"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of Rolling X Days Retentions</returns>
        [Route("GetUsersRollingRetentions/{days}")]
        [HttpGet]
        public async Task<IActionResult> GetUsersRollingRetentions([FromQuery] UserParameters userParameters, int days,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _usersRepository.GetUserList(cancellationToken, userParameters);
                if (users == null)
                {
                    return BadRequest(null);
                }

                var result = await _rollingRetention.ExecuteAsync(new CalculateRollingRetentionContext
                {
                    DaysCount = days,
                    Users = users,
                    CancellationToken = cancellationToken
                });
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