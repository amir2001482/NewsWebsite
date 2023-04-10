using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Common;
using NewsWebsite.Data.Contracts;
using NewsWebsite.Entities;
using NewsWebsite.ViewModels.Comments;
using NewsWebsite.ViewModels.DynamicAccess;
using NewsWebsite.ViewModels.Models;

namespace NewsWebsite.Areas.Admin.Controllers
{
    [DisplayName("مدیریت نظرات")]
    public class CommentsController : BaseController
    {
        private readonly IUnitOfWork _uw;
        private readonly IMapper _mapper;
        private const string CommentNotFound = "دیدگاه یافت نشد.";

        public CommentsController(IUnitOfWork uw, IMapper mapper)
        {
            _uw = uw;
            _uw.CheckArgumentIsNull(nameof(_uw));

            _mapper = mapper;
            _mapper.CheckArgumentIsNull(nameof(_mapper));
        }
        [DisplayName("مشاهده")]
        //[Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult Index(string newsId , bool? isConfirm)
        {
            return View(nameof(Index) , new CommentViewModel { NewsId = newsId , IsConfirm = isConfirm });
        }


        [HttpGet]
        public async Task<IActionResult> GetComments(string search, string order, int offset, int limit, string sort , string newsId , bool? isConfirm)
        {
            var model = new PaginateModel();
            List <CommentViewModel> comments;
            int total = _uw.BaseRepository<Comment>().CountEntities();
            if (!search.HasValue())
                search = "";
            if (!newsId.HasValue())
                newsId = "";

            if (limit == 0)
                limit = total;

            switch (sort)
            {
                case ("نام"):
                    if (order == "asc")
                        model.orderBy = "Name";
                    else
                        model.orderBy = "Name Desc";
                    break;
                case ("ایمیل"):
                    if (order == "asc")
                        model.orderBy = "Email";
                    else
                        model.orderBy = "Email Desc";
                    break;
                case ("تاریخ ارسال"):
                    if (order == "asc")
                        model.orderBy = "PostageDateTime";
                    else
                        model.orderBy = "PostageDateTime Desc";
                    break;
                default:
                    model.orderBy = "PostageDateTime";
                    break;
            }
            model.searchText = search;
            model.limit = limit;
            model.offset = offset;
            comments = await _uw.CommentRepository.GetPaginateCommentsAsync(model , newsId , isConfirm);

            if (search != "")
                total = comments.Count();

            return Json(new { total = total, rows = comments });
        }

        [HttpGet]
        [DisplayName("حذف")]
       //[Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> Delete(string commentId)
        {
            if (!commentId.HasValue())
                ModelState.AddModelError(string.Empty,CommentNotFound);
            else
            {
                var comment = await _uw.BaseRepository<Comment>().FindByIdAsync(commentId);
                if (comment == null)
                    ModelState.AddModelError(string.Empty, CommentNotFound);
                else
                    return PartialView("_DeleteConfirmation", comment);
            }
            return PartialView("_DeleteConfirmation");
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Comment model)
        {
            if (model.CommentId == null)
                ModelState.AddModelError(string.Empty,CommentNotFound);
            else
            {
                var comment = await _uw.BaseRepository<Comment>().FindByIdAsync(model.CommentId);
                if (comment == null)
                    ModelState.AddModelError(string.Empty,CommentNotFound);
                else
                {
                    _uw.BaseRepository<Comment>().Delete(comment);
                    await _uw.Commit();
                    TempData["notification"] = DeleteSuccess;
                    return PartialView("_DeleteConfirmation", comment);
                }
            }
            return PartialView("_DeleteConfirmation");
        }



        [HttpGet]
        [DisplayName("تایید یا رد")]
        //[Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public async Task<IActionResult> ConfirmOrInconfirm(string commentId)
        {
            if (!commentId.HasValue())
                ModelState.AddModelError(string.Empty, CommentNotFound);
            else
            {
                var comment = await _uw.BaseRepository<Comment>().FindByIdAsync(commentId);
                if (comment == null)
                    ModelState.AddModelError(string.Empty, CommentNotFound);
                else
                    return PartialView("_ConfirmOrInconfirm", comment);
            }
            return PartialView("_ConfirmOrInconfirm");
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmOrInconfirm(Comment model)
        {
            if (model.CommentId == null)
                ModelState.AddModelError(string.Empty, CommentNotFound);
            else
            {
                var comment = await _uw.BaseRepository<Comment>().FindByIdAsync(model.CommentId);
                if (comment == null)
                    ModelState.AddModelError(string.Empty, CommentNotFound);
                else
                {
                    if (comment.IsConfirm)
                        comment.IsConfirm = false;
                    else
                        comment.IsConfirm = true;

                    _uw.BaseRepository<Comment>().Update(comment);
                    await _uw.Commit();
                    TempData["notification"] = OperationSuccess;
                    return PartialView("_ConfirmOrInconfirm", comment);
                }
            }
            return PartialView("_ConfirmOrInconfirm");
        }


        [HttpPost, ActionName("DeleteGroup")]
        public async Task<IActionResult> DeleteGroupConfirmed(string[] btSelectItem)
        {
            if (btSelectItem.Count() == 0)
                ModelState.AddModelError(string.Empty, "هیچ دیدگاهی برای حذف انتخاب نشده است.");
            else
            {
                foreach (var item in btSelectItem)
                {
                    var comment = await _uw.BaseRepository<Comment>().FindByIdAsync(item);
                    _uw.BaseRepository<Comment>().Delete(comment);
                }

                await _uw.Commit();
                TempData["notification"] = "حذف گروهی اطلاعات با موفقیت انجام شد.";
            }

            return PartialView("_DeleteGroup");
        }

        [HttpGet]
        [DisplayName("ارسال نظر")]
        //[Authorize(Policy = ConstantPolicies.DynamicPermission)]
        public IActionResult SendComment(string parentCommentId, string newsId)
        {
            return PartialView("_SendComment",new CommentViewModel(parentCommentId, newsId));
        }

        [HttpPost]
        public async Task<IActionResult> SendComment(CommentViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    viewModel.PostageDateTime = DateTime.Now;
                    await _uw.BaseRepository<Comment>().CreateAsync(_mapper.Map<Comment>(viewModel));
                    await _uw.Commit();
                    TempData["notification"] = "دیدگاه شما با موفقیت ارسال شد و بعد از تایید در سایت نمایش داده می شود.";
                }
                return PartialView("_SendComment", viewModel);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
    }
}