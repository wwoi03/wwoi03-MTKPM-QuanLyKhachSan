﻿using Microsoft.EntityFrameworkCore;
using MTKPM_QuanLyKhachSan.Common;
using MTKPM_QuanLyKhachSan.Models;

namespace MTKPM_QuanLyKhachSan.Daos
{
    public class BookRoomDetailsDao
    {
        DatabaseContext context;

        public BookRoomDetailsDao(DatabaseContext context)
        {
            this.context = context;
        }

        // lấy danh sách phòng đặt chưa nhận
        public List<BookRoomDetails> GetBookRoomDetails()
        {
            return context.BookRoomDetails
                .Include(i => i.BookRoom.Customer)
                .Include(i => i.Room)
                .ToList();
        }

        // lấy danh sách phòng đã nhận
        public List<BookRoomDetails> GetBookRoomDetailsReceive(int? hotelId)
        {
            return context.BookRoomDetails
                .Include(i => i.BookRoom)
                .Include(i => i.BookRoom.Customer)
                .Include(i  => i.Room)
                .Include(i  => i.Room.RoomType)
                .Join(context.Rooms, 
                    brd => brd.RoomId, // Khóa ngoại từ BookRoomDetails
                    room => room.RoomId, // Khóa chính từ Room
                    (brd, room) => new { brd, room }) // Kết quả kết hợp)
                .Where(i => i.room.Status == (int)RoomStatusType.RoomOccupied && i.brd.HotelId == hotelId && (BookRoomDetailsType)i.brd.Status == BookRoomDetailsType.Received)
                .Select(i => i.brd)
                .ToList();
        }

        // tạo đặt phòng chi tiết
        public void AddBookRoomDetails(BookRoomDetails bookRoomDetails)
        {
            context.BookRoomDetails.Add(bookRoomDetails);
        }

        // lấy bookingDetails theo id
        public BookRoomDetails GetBookRoomDetailsById(int bookRoomDetailsId)
        {
            return context.BookRoomDetails
                .Where(i => i.BookRoomDetailsId == bookRoomDetailsId)
                .Include(i => i.BookRoom.Customer)
                .Include(i => i.Room)
                .Include(i => i.BookRoom)
                .FirstOrDefault();
        }

        // đổi phòng
        public void ChangeRoom(int bookRoomDetailsId, int roomIdNew)
        {
            BookRoomDetails bookRoomDetails = GetBookRoomDetailsById(bookRoomDetailsId);
            bookRoomDetails.RoomId = roomIdNew;
            context.Update(bookRoomDetails);
        }

        // cập nhật chi tiết đặt phòng
        public void UpdateBookRoomDetails(BookRoomDetails bookRoomDetails)
        {
            context.Update(bookRoomDetails);
        }

        // cập nhật chi tiết đặt phòng
        public void UpdateStatus(int bookRoomDetailsId, int status)
        {
            var bookRoomDetails = GetBookRoomDetailsById(bookRoomDetailsId);
            bookRoomDetails.Status = status;
            context.Update(bookRoomDetails);
        }

        // cập nhật ngày nhận phòng theo roomId
        public void UpdateCheckInByRoomId(int roomId)
        {
            BookRoomDetails? bookRoomDetails = context
                .BookRoomDetails
                .FirstOrDefault(i => i.RoomId == roomId && (BookRoomDetailsType)i.Status == BookRoomDetailsType.NotReceived);
            bookRoomDetails.Status = (int)BookRoomDetailsType.Received;
            bookRoomDetails.CheckIn = DateTime.Now;
        }

        // cập nhật ngày trả phòng
        public void UpdateCheckOut(int bookRoomDetailsId, DateTime checkOut)
        {
            var bookRoomDetails = GetBookRoomDetailsById(bookRoomDetailsId);
            bookRoomDetails.CheckOut = checkOut;
            context.Update(bookRoomDetails);
        }

        // hủy chi tiết đặt phòng
        public void CancelBookRoomDetailsByRoomId(int roomId)
        {
            BookRoomDetails? bookRoomDetails = context
                .BookRoomDetails
                .FirstOrDefault(i => i.RoomId == roomId && (BookRoomDetailsType)i.Status == BookRoomDetailsType.NotReceived);
            context.Remove(bookRoomDetails);
        }
    }
}
