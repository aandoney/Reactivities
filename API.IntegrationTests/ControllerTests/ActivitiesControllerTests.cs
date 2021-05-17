using API.DTOs;
using Application.Activities;
using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.FunctionalTests.ControllerTests
{
    public class ActivitiesControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private StringContent _httpContent;
        private List<ActivityDto> _result;
        private HttpResponseMessage _response;

        public ActivitiesControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GivenValidLogin_WhenGetActivities_ThenReturnList()
        {
            GivenValidCredentials();
            await WhenUserLogin();
            await ThenAuthenticateIsSuccessfully();

            await WhenGetActivitiesList();
            await ThenActivityListIsReturned();
        }

        [Fact]
        public async Task GivenUnAuthenticatedUser_WhenGetActivities_ReturnsUnauthorized()
        {
            GivenInvalidToken();
            await WhenGetActivitiesList();
            ThenResponseIsForbidden();
        }

        #region Givens

        private void GivenValidCredentials()
        {
            var json = JsonConvert.SerializeObject(new { email = "bob@test.com", password = "Pa$$w0rd" });
            _httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        }

        private void GivenInvalidToken()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Invalid Token");
        }

        #endregion

        #region Whens

        private async Task WhenUserLogin()
        {
            _response = await _client.PostAsync("/api/account/login", _httpContent);            
        }

        private async Task WhenGetActivitiesList()
        {
            _response = await _client.GetAsync("/api/activities");
        }

        #endregion

        #region Thens

        private async Task ThenAuthenticateIsSuccessfully()
        {
            _response.EnsureSuccessStatusCode();
            var stringResponse = await _response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<UserDto>(stringResponse);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
        }

        private async Task ThenActivityListIsReturned()
        {
            _response.EnsureSuccessStatusCode();
            var stringResponse = await _response.Content.ReadAsStringAsync();
            _result = JsonConvert.DeserializeObject<List<ActivityDto>>(stringResponse);
            _result.Count.Should().BeGreaterThan(0);
        }

        private void ThenResponseIsForbidden()
        {
            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        #endregion
    }
}
