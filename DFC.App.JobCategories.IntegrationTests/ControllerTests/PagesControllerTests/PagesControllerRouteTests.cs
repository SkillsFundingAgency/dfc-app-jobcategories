﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.JobCategories.IntegrationTests.ControllerTests.PagesControllerTests
{
    [Trait("Category", "Integration")]
    public class PagesControllerRouteTests : IClassFixture<CustomWebApplicationFactory<DFC.App.JobCategories.Startup>>
    {
        private readonly CustomWebApplicationFactory<DFC.App.JobCategories.Startup> factory;

        public PagesControllerRouteTests(CustomWebApplicationFactory<DFC.App.JobCategories.Startup> factory)
        {
            this.factory = factory;

            DataSeeding.SeedDefaultArticles(factory);
        }

        public static IEnumerable<object[]> PagesContentRouteData => new List<object[]>
        {
            new object[] { "/" },
            new object[] { "/pages" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}" },
            new object[] { $"/pages/head" },
            new object[] { $"/pages/breadcrumb" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/head" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/breadcrumb" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/body" },
        };

        public static IEnumerable<object[]> PagesNoContentRouteData => new List<object[]>
        {
            new object[] { $"/pages/bodytop" },
            new object[] { $"/pages/herobanner" },
            new object[] { $"/pages/sidebarright" },
            new object[] { $"/pages/sidebarleft" },
            new object[] { $"/pages/bodyfooter" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/bodytop" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/herobanner" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/sidebarright" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/sidebarleft" },
            new object[] { $"/pages/{DataSeeding.DefaultArticleName}/bodyfooter" },
            new object[] { $"/pages/bodytop" },
            new object[] { $"/pages/herobanner" },
            new object[] { $"/pages/sidebarright" },
            new object[] { $"/pages/sidebarleft" },
            new object[] { $"/pages/bodyfooter" },
            new object[] { $"/pages/{DataSeeding.AlternativeArticleName}/bodytop" },
            new object[] { $"/pages/{DataSeeding.AlternativeArticleName}/herobanner" },
            new object[] { $"/pages/{DataSeeding.AlternativeArticleName}/sidebarright" },
            new object[] { $"/pages/{DataSeeding.AlternativeArticleName}/sidebarleft" },
            new object[] { $"/pages/{DataSeeding.AlternativeArticleName}/bodyfooter" },
        };

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesHtmlContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
                // Arrange
                var uri = new Uri(url, UriKind.Relative);
                var client = factory.CreateClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));

                // Act
                var response = await client.GetAsync(uri).ConfigureAwait(false);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal($"{MediaTypeNames.Text.Html}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(PagesContentRouteData))]
        public async Task GetPagesJsonContentEndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal($"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.WebName}", response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [MemberData(nameof(PagesNoContentRouteData))]
        public async Task GetPagesEndpointsReturnSuccessAndNoContent(string url)
        {
            // Arrange
            var uri = new Uri(url, UriKind.Relative);
            var client = factory.CreateClient();
            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.GetAsync(uri).ConfigureAwait(false);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeletePagesEndpointsReturnNotFound()
        {
            // Arrange
            var uri = new Uri($"/pages/{Guid.NewGuid()}", UriKind.Relative);
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Accept.Clear();

            // Act
            var response = await client.DeleteAsync(uri).ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}