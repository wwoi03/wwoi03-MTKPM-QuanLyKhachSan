﻿using Microsoft.AspNetCore.Mvc;
using MTKPM_QuanLyKhachSan.Areas.Admin.DesignPattern.Facede;
using MTKPM_QuanLyKhachSan.Areas.Admin.DesignPattern.ProxyProtected.Services;
using MTKPM_QuanLyKhachSan.Daos;
using MTKPM_QuanLyKhachSan.Models;
using MTKPM_QuanLyKhachSan.ViewModels;
using Newtonsoft.Json;

namespace MTKPM_QuanLyKhachSan.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBookingController : Controller, IBooking
    {
        RoomDao roomDao;
        RoomTypeDao roomTypeDao;
        BookRoomDao bookRoomDao;
        BookRoomDetailsDao bookRoomDetailsDao;
        CustomerDao customerDao;
        BookingFacede bookingFacede;

        public AdminBookingController(DatabaseContext context)
        {
            roomDao = new RoomDao(context);
            roomTypeDao = new RoomTypeDao(context);
            bookRoomDao = new BookRoomDao(context);
            bookRoomDetailsDao = new BookRoomDetailsDao(context);
            customerDao = new CustomerDao(context);

            bookingFacede = new BookingFacede();
        }

        public IActionResult Index()
        {
            /*HttpContext.Session.SetInt32("EmployeeId", 1);
            HttpContext.Session.SetString("EmployeeName", "Đào Công Tuấn");
            HttpContext.Session.SetInt32("HotelId", 1);*/

            return View();
        }

        public IActionResult GetBooking()
        {
            var rooms = roomDao.GetRooms(HttpContext.Session.GetInt32("HotelId"));
            var bookings = bookRoomDetailsDao.GetBookRoomDetails();

            // Chuyển đổi từ Room sang RoomTitleVM
            List<RoomTitleVM> roomTitleVMs = rooms.Select(room => new RoomTitleVM
            {
                id = (room.RoomId).ToString(),
                title = room.Name,
            }).ToList();

            List<BookingEventVM> bookingEventVMs = bookings.Select(booking => new BookingEventVM
            {
                id = (booking.BookRoomDetailsId).ToString(),
                resourceId = (booking.RoomId).ToString(),
                start = DateTime.Parse(booking.CheckIn.ToString()).ToString("yyyy-MM-dd"),
                end = DateTime.Parse(booking.CheckOut.ToString()).ToString("yyyy-MM-dd"),
                title = booking.BookRoom.Customer.Name,
                color = "#2BA5F0",
            }).ToList();

            // Chuyển đổi danh sách RoomTitleVM sang chuỗi JSON
            string roomTitleVMJsons = JsonConvert.SerializeObject(roomTitleVMs, Formatting.Indented);
            string bookingEventVMsJsons = JsonConvert.SerializeObject(bookingEventVMs, Formatting.Indented);

            return Json(new
            {
                resources = roomTitleVMJsons,
                events = bookingEventVMsJsons,
            });
        }

        // Đặt phòng
        [HttpGet]
        public IActionResult Booking()
        {
            return PartialView("Booking", new BookingAdminVM());
        }

        [HttpPost]
        public IActionResult Booking(BookingAdminVM bookingAdminVM)
        {
            ExecutionOutcome executionOutcome = bookingFacede.Booking(bookingAdminVM);

            return Json(executionOutcome);
        }

        // chi tiết đặt phòng
        [HttpGet]
        public IActionResult BookingDetails(int bookRoomDetailsId)
        {
            BookRoomDetails bookRoomDetails = bookRoomDetailsDao.GetBookRoomDetailsById(bookRoomDetailsId);

            BookingAdminVM bookingDetailsVM = new BookingAdminVM()
            {
                BookRoomId = bookRoomDetails.BookRoomId,
                Name = bookRoomDetails.BookRoom.Customer.Name,
                Phone = bookRoomDetails.BookRoom.Customer.Phone,
                CheckIn = bookRoomDetails.CheckIn.ToString(),
                CheckOut = bookRoomDetails.CheckOut.ToString(),
                Note = bookRoomDetails.BookRoom.Note,
                CIC = bookRoomDetails.BookRoom.Customer.CIC,
                Rooms = roomDao.GetRooms(HttpContext.Session.GetInt32("HotelId"))
            };

            return PartialView("BookingDetails", bookingDetailsVM);
        }

        // sửa đặt phòng
        [HttpPost]
        public IActionResult EditBooking(BookingAdminVM bookingAdminVM)
        {
            ExecutionOutcome executionOutcome;
            string error;

            if (bookingAdminVM.Validation(out error) == true)
            {
                executionOutcome = new ViewModels.ExecutionOutcome()
                {
                    Result = true,
                    Mess = "Chỉnh sửa đặt phòng thành công.",
                };
            }
            else
            {
                executionOutcome = new ViewModels.ExecutionOutcome()
                {
                    Result = false,
                    Mess = error,
                };
            }

            return Json(executionOutcome);
        }

        // Chọn phòng đặt
        public IActionResult ChooseRoom()
        {
            ViewBag.rooms = roomDao.GetEmptyRooms(HttpContext.Session.GetInt32("HotelId"));
            ViewBag.roomTypes = roomTypeDao.GetRoomTypes(HttpContext.Session.GetInt32("HotelId"));

            return PartialView();
        }
    }
}
