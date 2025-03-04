﻿using Microsoft.EntityFrameworkCore;
using MTKPM_QuanLyKhachSan.Models;

namespace MTKPM_QuanLyKhachSan.Daos
{
    public class OrderDao
    {
        DatabaseContext context;

        public OrderDao(DatabaseContext context)
        {
            this.context = context;
        }

        // lấy số lượng order của một phòng
        public int CalcOrderQuantity(int bookRoomDetailsId)
		{
            return context.Orders
                .Where(i => i.BookRoomDetailsId == bookRoomDetailsId)
                .Sum(i => i.Quantity);
		}

        // tính tiền order của một phòng
        public decimal CalcOrderPrice(int bookRoomDetailsId)
		{
            return context.Orders
                .Where(i => i.BookRoomDetailsId == bookRoomDetailsId)
                .Sum(i => i.Quantity * i.Price);
        }

        // tạo order
        public void CreateOrder(Order order)
        {
            context.Orders.Add(order);
        }

        // lấy order theo bookRoomDetailsId
        public List<Order> GetOrderByBookRoomDetailsId(int bookRoomDetailsId)
        {
            return context.Orders
                .Include(i => i.BookRoomDetails)
                .Include(i => i.Service)
                .Where(i => i.BookRoomDetailsId == bookRoomDetailsId)
                .ToList();
        }
    }
}
