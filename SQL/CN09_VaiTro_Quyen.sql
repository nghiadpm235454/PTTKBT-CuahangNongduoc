-- CN-09: bảng VAI_TRO + QUYEN (vai trò -> chức năng) cho đăng nhập & phân quyền.
-- Chạy SAU script CuahangNongduoc_Full.sql (cần bảng NGUOI_DUNG). Chạy lại nhiều lần vẫn an toàn.
-- Không chạy script này thì chương trình vẫn chạy bằng bảng quyền mặc định trong Security/PhanQuyen.cs.
USE CuahangNongduoc;
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'VAI_TRO')
BEGIN
    CREATE TABLE VAI_TRO (
        MA_VAI_TRO  NVARCHAR(20)  NOT NULL PRIMARY KEY,   -- trùng giá trị cột NGUOI_DUNG.ROLE
        TEN_VAI_TRO NVARCHAR(100) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'QUYEN')
BEGIN
    CREATE TABLE QUYEN (
        MA_VAI_TRO   NVARCHAR(20) NOT NULL REFERENCES VAI_TRO(MA_VAI_TRO),
        MA_CHUC_NANG NVARCHAR(50) NOT NULL,               -- trùng hằng số trong Security/PhanQuyen.cs (ChucNang)
        CONSTRAINT PK_QUYEN PRIMARY KEY (MA_VAI_TRO, MA_CHUC_NANG)
    );
END
GO

-- Vai trò
MERGE VAI_TRO AS t
USING (VALUES ('admin', N'Quản trị'), ('banhang', N'Nhân viên bán hàng'), ('thukho', N'Thủ kho')) AS s (MA, TEN)
ON t.MA_VAI_TRO = s.MA
WHEN NOT MATCHED THEN INSERT (MA_VAI_TRO, TEN_VAI_TRO) VALUES (s.MA, s.TEN);
GO

-- Quyền (giống bảng mặc định trong code)
MERGE QUYEN AS t
USING (VALUES
    ('admin','QL_LYDOCHI'),('admin','QL_DONVITINH'),('admin','QL_SANPHAM'),('admin','QL_KHACHHANG'),
    ('admin','QL_DAILY'),('admin','QL_NHACUNGCAP'),('admin','NHAP_HANG'),('admin','BAN_LE'),('admin','BAN_SI'),
    ('admin','PHIEU_CHI'),('admin','PHIEU_THU'),('admin','TONGHOP_DUNO'),('admin','BC_TON'),('admin','BC_BAN'),
    ('admin','BC_HETHAN'),('admin','CAU_HINH'),('admin','TRO_GIUP'),
    ('banhang','BAN_LE'),('banhang','BAN_SI'),('banhang','QL_KHACHHANG'),('banhang','QL_DAILY'),
    ('banhang','PHIEU_THU'),('banhang','TONGHOP_DUNO'),('banhang','BC_BAN'),('banhang','BC_TON'),('banhang','TRO_GIUP'),
    ('thukho','NHAP_HANG'),('thukho','QL_SANPHAM'),('thukho','QL_DONVITINH'),('thukho','QL_NHACUNGCAP'),
    ('thukho','BC_TON'),('thukho','BC_HETHAN'),('thukho','TRO_GIUP')
) AS s (VT, CN)
ON t.MA_VAI_TRO = s.VT AND t.MA_CHUC_NANG = s.CN
WHEN NOT MATCHED THEN INSERT (MA_VAI_TRO, MA_CHUC_NANG) VALUES (s.VT, s.CN);
GO

-- Tài khoản demo để thử phân quyền (mật khẩu 123456 lưu thường; chương trình tự băm PBKDF2 + SALT
-- ngay lần đăng nhập đúng đầu tiên rồi ghi đè lại vào MATKHAU/SALT).
IF NOT EXISTS (SELECT * FROM NGUOI_DUNG WHERE TEN_DANG_NHAP = 'banhang')
    INSERT INTO NGUOI_DUNG (TEN_DANG_NHAP, MATKHAU, SALT, ROLE, HOTEN, IS_ACTIVE, MUST_CHANGE_PWD)
    VALUES ('banhang', '123456', NULL, 'banhang', N'Nhân viên bán hàng', 1, 0);
IF NOT EXISTS (SELECT * FROM NGUOI_DUNG WHERE TEN_DANG_NHAP = 'thukho')
    INSERT INTO NGUOI_DUNG (TEN_DANG_NHAP, MATKHAU, SALT, ROLE, HOTEN, IS_ACTIVE, MUST_CHANGE_PWD)
    VALUES ('thukho', '123456', NULL, 'thukho', N'Thủ kho', 1, 0);
GO

SELECT MA_VAI_TRO, COUNT(*) AS SO_QUYEN FROM QUYEN GROUP BY MA_VAI_TRO;
SELECT ID, TEN_DANG_NHAP, ROLE, HOTEN, CASE WHEN SALT IS NULL THEN N'văn bản thường' ELSE N'đã băm' END AS MAT_KHAU FROM NGUOI_DUNG;
GO
