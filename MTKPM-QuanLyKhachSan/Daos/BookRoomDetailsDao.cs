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
        public List<BookRoomDetails> GetBookRoomDetailsReceive()
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
                .Where(i => (RoomStatusType)i.brd.Status == RoomStatusType.RoomOccupied)
                .Select(i => i.brd)
                .ToList();
        }

        // tạo đặt phòng chi tiết
        public void AddBookRoomDetails(BookRoomDetails bookRoomDetails)
        {
            context.BookRoomDetails.Add(bookRoomDetails);
            context.SaveChanges();
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
        public void ChangeRoom(int roomIdOld, int roomIdNew)
        {
            BookRoomDetails bookRoomDetails = GetBookRoomDetailsById(roomIdOld);
            bookRoomDetails.RoomId = roomIdNew;
            context.Update(bookRoomDetails);
            context.SaveChanges();
        }

        // cập nhật chi tiết đặt phòng
        public void UpdateBookRoomDetails(BookRoomDetails bookRoomDetails)
        {
            context.Update(bookRoomDetails);
            context.SaveChanges();
        }
    }
}
