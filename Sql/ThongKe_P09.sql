/* =====================================================================
   P09 - Thư | CN-07 + CN-08 + FIX-08
   Thống kê – truy vấn: tồn kho, vận chuyển, dịch vụ phụ, giảm giá /
   khuyến mãi, theo nhân viên, tổng hợp chứng từ.
   CHẠY SAU: CuahangNongduoc_Full__2_.sql  (cần PHIEU_BAN đã có các cột
   TIEN_GIAM_GIA, PHI_VAN_CHUYEN, ID_NGUOI_DUNG... và bảng NGUOI_DUNG,
   CHI_TIET_DICH_VU_PHIEU_BAN).
   Chạy lại nhiều lần được (DROP rồi CREATE).
   GIẢ ĐỊNH CẦN XÁC NHẬN VỚI LINH (P05): PHIEU_BAN.LOAI_GIAM_GIA dùng
   'GIAM_GIA' (giảm giá trực tiếp) và 'KHUYEN_MAI' (khuyến mãi).
   ===================================================================== */
USE [CuahangNongduoc];
GO

/* ---------- 1. TỒN KHO ----------
   Trước: SanPhamFactory.LaySoLuongTon dùng INNER JOIN -> sản phẩm chưa có
          lô bị mất khỏi báo cáo.
   Sau  : LEFT JOIN + ISNULL(SUM(...),0)                                  */
IF OBJECT_ID(N'dbo.vw_TonKho', N'V') IS NOT NULL DROP VIEW dbo.vw_TonKho;
GO
CREATE VIEW dbo.vw_TonKho AS
SELECT  SP.ID,
        SP.TEN_SAN_PHAM,
        DVT.TEN_DON_VI,
        ISNULL(SUM(MA.SO_LUONG), 0)                                  AS SO_LUONG_TON,
        COUNT(CASE WHEN MA.SO_LUONG > 0 THEN 1 END)                  AS SO_LO_CON,
        MIN(CASE WHEN MA.SO_LUONG > 0 THEN MA.NGAY_HET_HAN END)      AS HSD_GAN_NHAT,
        SP.GIA_BAN_SI,
        SP.GIA_BAN_LE
FROM    dbo.SAN_PHAM SP
        LEFT JOIN dbo.MA_SAN_PHAM  MA  ON MA.ID_SAN_PHAM = SP.ID
        LEFT JOIN dbo.DON_VI_TINH  DVT ON DVT.ID = SP.ID_DON_VI_TINH
GROUP BY SP.ID, SP.TEN_SAN_PHAM, DVT.TEN_DON_VI, SP.GIA_BAN_SI, SP.GIA_BAN_LE;
GO

/* ---------- 2. CHI PHÍ VẬN CHUYỂN (từ ngày - đến ngày) ----------
   Lọc ngày theo khoảng [@tu, @den + 1 ngày)  (R32)
   SUM/ cột có thể NULL -> ISNULL (R29)                                    */
IF OBJECT_ID(N'dbo.sp_ChiPhiVanChuyen', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_ChiPhiVanChuyen;
GO
CREATE PROCEDURE dbo.sp_ChiPhiVanChuyen
    @tu  DATETIME,
    @den DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  PB.ID                              AS ID_PHIEU_BAN,
            PB.NGAY_BAN,
            KH.HO_TEN                          AS KHACH_HANG,
            ISNULL(PB.KHOANG_CACH_KM, 0)       AS KHOANG_CACH_KM,
            ISNULL(PB.PHI_VAN_CHUYEN, 0)       AS PHI_VAN_CHUYEN
    FROM    dbo.PHIEU_BAN PB
            LEFT JOIN dbo.KHACH_HANG KH ON KH.ID = PB.ID_KHACH_HANG
    WHERE   PB.NGAY_BAN >= @tu
        AND PB.NGAY_BAN <  DATEADD(DAY, 1, @den)
        AND ISNULL(PB.PHI_VAN_CHUYEN, 0) > 0
    ORDER BY PB.NGAY_BAN, PB.ID;
END
GO

/* ---------- 3. DỊCH VỤ PHỤ (từ ngày - đến ngày) ---------- */
IF OBJECT_ID(N'dbo.sp_DichVuPhu', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_DichVuPhu;
GO
CREATE PROCEDURE dbo.sp_DichVuPhu
    @tu  DATETIME,
    @den DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  PB.ID                    AS ID_PHIEU_BAN,
            PB.NGAY_BAN,
            DV.TEN_DICH_VU,
            ISNULL(DV.SO_TIEN, 0)    AS SO_TIEN
    FROM    dbo.CHI_TIET_DICH_VU_PHIEU_BAN DV
            INNER JOIN dbo.PHIEU_BAN PB ON PB.ID = DV.ID_PHIEU_BAN
    WHERE   PB.NGAY_BAN >= @tu
        AND PB.NGAY_BAN <  DATEADD(DAY, 1, @den)
    ORDER BY PB.NGAY_BAN, PB.ID, DV.ID;
END
GO

/* ---------- 4. GIẢM GIÁ / KHUYẾN MÃI (từ ngày - đến ngày) ----------
   @loai = NULL      : lấy cả giảm giá lẫn khuyến mãi
   @loai = 'GIAM_GIA': chỉ giảm giá trực tiếp
   @loai = 'KHUYEN_MAI': chỉ khuyến mãi                                    */
IF OBJECT_ID(N'dbo.sp_GiamGia', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_GiamGia;
GO
CREATE PROCEDURE dbo.sp_GiamGia
    @tu   DATETIME,
    @den  DATETIME,
    @loai NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  PB.ID                            AS ID_PHIEU_BAN,
            PB.NGAY_BAN,
            KH.HO_TEN                        AS KHACH_HANG,
            PB.LOAI_GIAM_GIA,
            ISNULL(PB.TY_LE_GIAM_GIA, 0)     AS TY_LE_GIAM_GIA,
            ISNULL(PB.TIEN_GIAM_GIA, 0)      AS TIEN_GIAM_GIA,
            ISNULL(PB.TONG_TIEN, 0)          AS TONG_TIEN
    FROM    dbo.PHIEU_BAN PB
            LEFT JOIN dbo.KHACH_HANG KH ON KH.ID = PB.ID_KHACH_HANG
    WHERE   PB.NGAY_BAN >= @tu
        AND PB.NGAY_BAN <  DATEADD(DAY, 1, @den)
        AND ISNULL(PB.TIEN_GIAM_GIA, 0) > 0
        AND (@loai IS NULL OR PB.LOAI_GIAM_GIA = @loai)
    ORDER BY PB.NGAY_BAN, PB.ID;
END
GO

/* ---------- 5. GIẢM GIÁ / KHUYẾN MÃI THEO NHÂN VIÊN (CN-08) ----------
   @idNguoiDung = NULL : tất cả nhân viên
   @idNguoiDung = n    : chỉ nhân viên đang đăng nhập (NGUOI_DUNG.ID)      */
IF OBJECT_ID(N'dbo.sp_GiamGiaTheoNhanVien', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_GiamGiaTheoNhanVien;
GO
CREATE PROCEDURE dbo.sp_GiamGiaTheoNhanVien
    @tu          DATETIME,
    @den         DATETIME,
    @idNguoiDung INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT  ND.ID                            AS ID_NGUOI_DUNG,
            ISNULL(ND.HOTEN, ND.TEN_DANG_NHAP) AS NHAN_VIEN,
            PB.ID                            AS ID_PHIEU_BAN,
            PB.NGAY_BAN,
            PB.LOAI_GIAM_GIA,
            ISNULL(PB.TY_LE_GIAM_GIA, 0)     AS TY_LE_GIAM_GIA,
            ISNULL(PB.TIEN_GIAM_GIA, 0)      AS TIEN_GIAM_GIA,
            ISNULL(PB.TONG_TIEN, 0)          AS TONG_TIEN
    FROM    dbo.PHIEU_BAN PB
            INNER JOIN dbo.NGUOI_DUNG ND ON ND.ID = PB.ID_NGUOI_DUNG
    WHERE   PB.NGAY_BAN >= @tu
        AND PB.NGAY_BAN <  DATEADD(DAY, 1, @den)
        AND ISNULL(PB.TIEN_GIAM_GIA, 0) > 0
        AND (@idNguoiDung IS NULL OR ND.ID = @idNguoiDung)
    ORDER BY ND.ID, PB.NGAY_BAN, PB.ID;
END
GO

/* ---------- 6. TỔNG HỢP NHIỀU LOẠI CHỨNG TỪ (cho Visitor) ----------
   LOAI: PHIEU_BAN, PHIEU_THANH_TOAN (thu) | PHIEU_NHAP, PHIEU_CHI (chi)   */
IF OBJECT_ID(N'dbo.sp_TongHopChungTu', N'P') IS NOT NULL DROP PROCEDURE dbo.sp_TongHopChungTu;
GO
CREATE PROCEDURE dbo.sp_TongHopChungTu
    @tu  DATETIME,
    @den DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @denTruoc DATETIME = DATEADD(DAY, 1, @den);

    SELECT N'PHIEU_BAN'       AS LOAI, ID, NGAY_BAN       AS NGAY, ISNULL(TONG_TIEN, 0) AS TONG_TIEN
    FROM dbo.PHIEU_BAN       WHERE NGAY_BAN       >= @tu AND NGAY_BAN       < @denTruoc
    UNION ALL
    SELECT N'PHIEU_THANH_TOAN', ID, NGAY_THANH_TOAN,       ISNULL(TONG_TIEN, 0)
    FROM dbo.PHIEU_THANH_TOAN WHERE NGAY_THANH_TOAN >= @tu AND NGAY_THANH_TOAN < @denTruoc
    UNION ALL
    SELECT N'PHIEU_NHAP',      ID, NGAY_NHAP,              ISNULL(TONG_TIEN, 0)
    FROM dbo.PHIEU_NHAP      WHERE NGAY_NHAP      >= @tu AND NGAY_NHAP      < @denTruoc
    UNION ALL
    SELECT N'PHIEU_CHI',       ID, NGAY_CHI,               ISNULL(TONG_TIEN, 0)
    FROM dbo.PHIEU_CHI       WHERE NGAY_CHI       >= @tu AND NGAY_CHI       < @denTruoc
    ORDER BY NGAY, LOAI, ID;
END
GO

PRINT N'Đã tạo xong: vw_TonKho, sp_ChiPhiVanChuyen, sp_DichVuPhu, sp_GiamGia, sp_GiamGiaTheoNhanVien, sp_TongHopChungTu';
GO
