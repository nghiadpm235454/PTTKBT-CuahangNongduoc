/* P09 - Dữ liệu thử cho thống kê (chạy SAU ThongKe_P09.sql).
   Tạo 3 phiếu bán DEMO có vận chuyển / dịch vụ / giảm giá / khuyến mãi.
   Chạy lại nhiều lần được. Xóa bằng cách chạy phần DELETE ở đầu file.  */
USE [CuahangNongduoc];
GO
SET NOCOUNT ON;

DELETE FROM dbo.CHI_TIET_DICH_VU_PHIEU_BAN WHERE ID_PHIEU_BAN IN ('DEMO-PB2','DEMO-PB3','DEMO-PB4');
DELETE FROM dbo.PHIEU_BAN                  WHERE ID IN ('DEMO-PB2','DEMO-PB3','DEMO-PB4');

DECLARE @admin INT = (SELECT ID FROM dbo.NGUOI_DUNG WHERE TEN_DANG_NHAP = 'admin');
DECLARE @nhom6 INT = (SELECT ID FROM dbo.NGUOI_DUNG WHERE TEN_DANG_NHAP = 'nhom6');
DECLARE @hn DATETIME = CAST(GETDATE() AS DATE);

-- Giảm giá trực tiếp 10% + giao hàng 12 km + 1 dịch vụ (admin)
INSERT INTO dbo.PHIEU_BAN
 (ID, ID_KHACH_HANG, NGAY_BAN, TONG_TIEN, DA_TRA, CON_NO, TIEN_HANG, TONG_DICH_VU,
  GIAO_HANG, KHOANG_CACH_KM, PHI_VAN_CHUYEN, LOAI_GIAM_GIA, TY_LE_GIAM_GIA, TIEN_GIAM_GIA, ID_NGUOI_DUNG)
VALUES
 ('DEMO-PB2','DEMO-DL1', DATEADD(DAY,-3,@hn), 1010000, 1010000, 0, 1000000, 50000,
  1, 12, 60000, 'GIAM_GIA', 10, 100000, @admin);
INSERT INTO dbo.CHI_TIET_DICH_VU_PHIEU_BAN (ID_PHIEU_BAN, TEN_DICH_VU, SO_TIEN)
VALUES ('DEMO-PB2', N'Tư vấn kỹ thuật tại ruộng', 50000);

-- Khuyến mãi 5% (admin)
INSERT INTO dbo.PHIEU_BAN
 (ID, ID_KHACH_HANG, NGAY_BAN, TONG_TIEN, DA_TRA, CON_NO, TIEN_HANG, TONG_DICH_VU,
  GIAO_HANG, KHOANG_CACH_KM, PHI_VAN_CHUYEN, LOAI_GIAM_GIA, TY_LE_GIAM_GIA, TIEN_GIAM_GIA, ID_NGUOI_DUNG)
VALUES
 ('DEMO-PB3','DEMO-KL1', DATEADD(DAY,-2,@hn), 570000, 570000, 0, 600000, 0,
  0, 0, 0, 'KHUYEN_MAI', 5, 30000, @admin);

-- Giảm giá 20.000đ + dịch vụ đóng gói + vận chuyển (nhom6)
INSERT INTO dbo.PHIEU_BAN
 (ID, ID_KHACH_HANG, NGAY_BAN, TONG_TIEN, DA_TRA, CON_NO, TIEN_HANG, TONG_DICH_VU,
  GIAO_HANG, KHOANG_CACH_KM, PHI_VAN_CHUYEN, LOAI_GIAM_GIA, TY_LE_GIAM_GIA, TIEN_GIAM_GIA, ID_NGUOI_DUNG)
VALUES
 ('DEMO-PB4','DEMO-DL1', DATEADD(DAY,-1,@hn), 355000, 0, 355000, 300000, 25000,
  1, 5, 50000, 'GIAM_GIA', 0, 20000, @nhom6);
INSERT INTO dbo.CHI_TIET_DICH_VU_PHIEU_BAN (ID_PHIEU_BAN, TEN_DICH_VU, SO_TIEN)
VALUES ('DEMO-PB4', N'Đóng gói', 25000);
GO

-- Kiểm tra nhanh (kỳ vọng: có dòng ở cả 5 câu)
DECLARE @tu DATETIME = DATEADD(DAY,-30,GETDATE()), @den DATETIME = GETDATE();
SELECT * FROM dbo.vw_TonKho;
EXEC dbo.sp_ChiPhiVanChuyen       @tu, @den;
EXEC dbo.sp_DichVuPhu             @tu, @den;
EXEC dbo.sp_GiamGia               @tu, @den, NULL;
EXEC dbo.sp_GiamGiaTheoNhanVien   @tu, @den, NULL;
EXEC dbo.sp_TongHopChungTu        @tu, @den;
GO
