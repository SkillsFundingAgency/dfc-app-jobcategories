﻿using DFC.App.JobCategories.Data.Models;
using DFC.App.JobCategories.Extensions;
using DFC.App.JobCategories.PageService;
using DFC.App.JobCategories.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.JobCategories.Controllers
{
    public class PagesController : Controller
    {
        public const string RegistrationPath = "job-categories";
        public const string LocalPath = "pages";

        private readonly ILogger<PagesController> logger;
        private readonly IDocumentService<JobCategory> documentService;
        private readonly AutoMapper.IMapper mapper;

        public PagesController(ILogger<PagesController> logger, IDocumentService<JobCategory> documentService, AutoMapper.IMapper mapper)
        {
            this.logger = logger;
            this.documentService = documentService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("/")]
        [Route("pages")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel()
            {
                LocalPath = LocalPath,
            };
            var contentPageModels = await documentService.GetAllAsync().ConfigureAwait(false);

            if (contentPageModels != null)
            {
                viewModel.Documents = (from a in contentPageModels.OrderBy(o => o.CanonicalName)
                                       select mapper.Map<IndexDocumentViewModel>(a)).ToList();
                logger.LogInformation($"{nameof(Index)} has succeeded");
            }
            else
            {
                logger.LogWarning($"{nameof(Index)} has returned with no results");
            }

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{jobCategory}")]
        public async Task<IActionResult> Document(string jobCategory)
        {
            var contentPageModel = await GetContentPageAsync(jobCategory).ConfigureAwait(false);

            if (contentPageModel == null)
            {
                return NoContent();
            }

            //var jpsToRetrieveHrefs = contentPageModel.Links?.Where(x => x.LinkValue.Key == nameof(JobProfile).ToLower()).Select(z => z.LinkValue.Value.Href);
            //var jobProfiles = jpsToRetrieveHrefs?.Select(x => jobProfilePageContentService.GetByUriAsync(x));

            var viewModel = mapper.Map<DocumentViewModel>(contentPageModel);

            viewModel.Breadcrumb = BuildBreadcrumb(contentPageModel);

            logger.LogInformation($"{nameof(Document)} has succeeded for: {jobCategory}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{article}/head")]
        [Route("pages/head")]
        public async Task<IActionResult> Head(string? article)
        {
            var viewModel = new HeadViewModel();
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);

            if (contentPageModel != null)
            {
                mapper.Map(contentPageModel, viewModel);

                viewModel.CanonicalUrl = new Uri($"{Request.GetBaseAddress()}{RegistrationPath}/{contentPageModel.CanonicalName}", UriKind.RelativeOrAbsolute);
            }

            logger.LogInformation($"{nameof(Head)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [Route("pages/{article}/breadcrumb")]
        [Route("pages/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string? article)
        {
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);
            var viewModel = BuildBreadcrumb(contentPageModel);

            logger.LogInformation($"{nameof(Breadcrumb)} has returned content for: {article}");

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{article}/bodytop")]
        [Route("pages/bodytop")]
        public IActionResult BodyTop(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/herobanner")]
        [Route("pages/herobanner")]
        public IActionResult HeroBanner(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/body")]
        [Route("pages/body")]
        public async Task<IActionResult> Body(string? article)
        {
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);

            if (contentPageModel == null)
            {
                return NotFound();
            }

            if (contentPageModel.JobProfiles == null || !contentPageModel.JobProfiles.Any())
            {
                return NoContent();
            }

            var viewModel = new BodyViewModel
            {
                Category = contentPageModel.Title,
                Profiles = contentPageModel.JobProfiles
                    .Where(x => x != null)
                    .Select(x => new JobProfileListItemViewModel(x.Title!, x.Uri.ToString(), x.Occupation?.OccupationLabels?.Select(l => l.Title!) ?? null, x.Description!)),
            };

            return this.NegotiateContentResult(viewModel, contentPageModel);
        }

        [HttpGet]
        [Route("pages/{article}/sidebarright")]
        [Route("pages/sidebarright")]
        public async Task<IActionResult> SidebarRight(string? article)
        {
            var contentPageModel = await GetContentPageAsync(article).ConfigureAwait(false);

            if (contentPageModel == null)
            {
                return NoContent();
            }

            var viewModel = new SidebarRightViewModel();
            var categories = await documentService.GetAllAsync().ConfigureAwait(false);

            viewModel.Categories = categories
                .Where(x => x.CanonicalName != article)
                .OrderBy(x => x.Title)
                .ToDictionary(x => x.Title!, x => x.CanonicalName!);

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("pages/{article}/sidebarleft")]
        [Route("pages/sidebarleft")]
        public IActionResult SidebarLeft(string? article)
        {
            return NoContent();
        }

        [HttpGet]
        [Route("pages/{article}/bodyfooter")]
        [Route("pages/bodyfooter")]
        public IActionResult BodyFooter(string? article)
        {
            return NoContent();
        }

        [HttpPost]
        [Route("pages")]
        public async Task<IActionResult> Create([FromBody]JobCategory? upsertContentPageModel)
        {
            if (upsertContentPageModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDocument = await documentService.GetByIdAsync(upsertContentPageModel.Id).ConfigureAwait(false);
            if (existingDocument != null)
            {
                return new StatusCodeResult((int)HttpStatusCode.AlreadyReported);
            }

            var response = await documentService.UpsertAsync(upsertContentPageModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Create)} has upserted content for: {upsertContentPageModel.CanonicalName} with response code {response}");

            return new StatusCodeResult((int)response);
        }

        [HttpPut]
        [Route("pages")]
        public async Task<IActionResult> Update([FromBody]JobCategory? upsertContentPageModel)
        {
            if (upsertContentPageModel == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingDocument = await documentService.GetByIdAsync(upsertContentPageModel.Id).ConfigureAwait(false);
            if (existingDocument == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            upsertContentPageModel.Etag = existingDocument.Etag;

            var response = await documentService.UpsertAsync(upsertContentPageModel).ConfigureAwait(false);

            logger.LogInformation($"{nameof(Update)} has upserted content for: {upsertContentPageModel.CanonicalName} with response code {response}");

            return new StatusCodeResult((int)response);
        }

        [HttpDelete]
        [Route("pages/{documentId}")]
        public async Task<IActionResult> Delete(Guid documentId)
        {
            if (await documentService.DeleteAsync(documentId).ConfigureAwait(false))
            {
                logger.LogInformation($"{nameof(Delete)} has deleted content for document Id: {documentId}");
                return Ok();
            }
            else
            {
                logger.LogWarning($"{nameof(Delete)} has returned no content for: {documentId}");
                return NotFound();
            }
        }

        //Not sure where this belongs really so plonking it here for now!
        [HttpGet("pages/explore-careers")]
        public async Task<IActionResult> ExploreCareers()
        {
            var categories = await documentService.GetAllAsync().ConfigureAwait(false);

            if (categories == null)
            {
                return NoContent();
            }

            return this.NegotiateContentResult(categories);
        }

        #region Define helper methods

        private static BreadcrumbViewModel BuildBreadcrumb(JobCategory? jobCategoryModel)
        {
            var viewModel = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>()
                {
                    new BreadcrumbPathViewModel()
                    {
                        Route = "/explore-careers",
                        Title = "Home: Explore careers",
                    },
                },
            };

            if (jobCategoryModel != null)
            {
                if (!string.IsNullOrWhiteSpace(jobCategoryModel.CanonicalName))
                {
                    var jobCategoryPathViewModel = new BreadcrumbPathViewModel
                    {
                        Route = $"/{jobCategoryModel.CanonicalName}",
                        Title = $"{jobCategoryModel.Title}",
                    };

                    viewModel.Paths.Add(jobCategoryPathViewModel);
                }

                viewModel.Paths.Last().AddHyperlink = false;
            }

            return viewModel;
        }

        private async Task<JobCategory?> GetContentPageAsync(string? article)
        {
            const string defaultArticleName = "home";
            var articleName = string.IsNullOrWhiteSpace(article) ? defaultArticleName : article;
            return (await documentService.GetAsync(x => x.CanonicalName == articleName).ConfigureAwait(false))?.SingleOrDefault() ?? null;
        }

        #endregion Define helper methods
    }
}