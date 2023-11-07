using Digital_Library.Domain.Entities;
using Digital_Library.Domain.Services;
using Digital_Library.Infrastructure;
using Digital_Library.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection.PortableExecutable;

namespace Digital_Library.Controllers
{
    public class BooksController :Controller
    {
        private readonly IBooksReader _reader;
        private readonly IBooksService _booksService;
        private readonly IWebHostEnvironment _appEnvironment;

        public BooksController (IBooksReader reader, IBooksService booksService, IWebHostEnvironment appEnvironment)
        {
            _reader = reader;
            _booksService = booksService;
            _appEnvironment = appEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index (string searchString = "", int categoryId = 0)
        {
            var viewModel = new BooksCatalogViewModel
            {
                Books = await _reader.FindBooksAsync(searchString, categoryId),
                Categories = await _reader.GetCategoriesAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBook ()
        {
            var viewModel = new BookViewModel();

            // загружаем список категорий (List<Category>)
            var categories = await _reader.GetCategoriesAsync();

            // получаем элементы для <select> с помощью нашего листа категорий
            // (List<SelectListItem>)
            var items = categories.Select(c =>
                new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            // добавляем список в модель представления
            viewModel.Categories.AddRange(items);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBook (BookViewModel bookVm)
        {
            if (!ModelState.IsValid)
            {
                return View(bookVm);
            }

            try
            {
                var book = new Book
                {
                    Author = bookVm.Author,
                    Title = bookVm.Title,
                    CategoryId = bookVm.CategoryId,
                    PagesCount = bookVm.PageCount,
                    Description = bookVm.Description
                };
                string wwwroot = _appEnvironment.WebRootPath; // получаем путь до wwwroot

                book.Filename =
                    await _booksService.LoadFile(bookVm.File.OpenReadStream(), Path.Combine(wwwroot, "books"));
                book.ImageUrl =
                    await _booksService.LoadPhoto(bookVm.Photo.OpenReadStream(), Path.Combine(wwwroot, "images", "books"));
                await _booksService.AddBook(book);
            } catch (IOException)
            {
                ModelState.AddModelError("ioerror", "Не удалось сохранить файл.");
                return View(bookVm);
            } catch
            {
                ModelState.AddModelError("database", "Ошибка при сохранении в базу данных.");
                return View(bookVm);
            }

            return RedirectToAction("Index", "Books");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBook (int bookId)
        {
            var book = await _reader.FindBookAsync(bookId);
            if (book is null)
            {
                return NotFound();
            }
            var bookVm = new UpdateBookViewModel
            {
                Id = book.Id,
                Author = book.Author,
                Title = book.Title,
                CategoryId = book.CategoryId,
                PageCount = book.PagesCount,
                FileString = book.Filename,
                PhotoString = book.ImageUrl,
                Description = book.Description
            };

            var categories = await _reader.GetCategoriesAsync();
            var items = categories.Select(c =>
                new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            bookVm.Categories.AddRange(items);

            return View(bookVm);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBook (UpdateBookViewModel bookVm)
        {
            // если модель не валидна, то возвращаем пользователя на форму
            if (!ModelState.IsValid)
            {
                return View(bookVm);
            }
            // находим книгу по Id
            var book = await _reader.FindBookAsync(bookVm.Id);
            // если книга почему-то не найдена, то выведем сообщение
            if (book is null)
            {
                ModelState.AddModelError("not_found", "Книга не найдена!");
                return View(bookVm);
            }

            try
            {
                // заполняем поля книги
                book.Author = bookVm.Author;
                book.CategoryId = bookVm.CategoryId;
                book.Title = bookVm.Title;
                book.PagesCount = bookVm.PageCount;
                // получаем путь до wwwroot
                string wwwroot = _appEnvironment.WebRootPath;
                // если формой был передан файл, то меняем его
                if (bookVm.File is not null)
                {
                    book.Filename = await _booksService.LoadFile(
                            bookVm.File.OpenReadStream(),
                            Path.Combine(wwwroot, "books")
                    );
                }
                // если формой было передано изображение, то меняем его
                if (bookVm.Photo is not null)
                {
                    book.ImageUrl = await _booksService.LoadPhoto(
                            bookVm.Photo.OpenReadStream(),
                            Path.Combine(wwwroot, "images", "books")
                    );
                }
                // обновляем файл
                await _booksService.UpdateBook(book);
            } catch (IOException)
            {
                ModelState.AddModelError("ioerror", "Не удалось сохранить файл.");
                return View(bookVm);
            } catch
            {
                ModelState.AddModelError("database", "Ошибка при сохранении в базу данных.");
                return View(bookVm);
            }

            return RedirectToAction("Index", "Books");
        }
    }
}
