/*
Navicat MySQL Data Transfer

Source Server         : 127.0.0.1_3306
Source Server Version : 50528
Source Host           : 127.0.0.1:3306
Source Database       : DosORMMySql

Target Server Type    : MYSQL
Target Server Version : 50528
File Encoding         : 65001

Date: 2015-08-28 10:28:44
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for Table2Mysql
-- ----------------------------
DROP TABLE IF EXISTS `Table2Mysql`;
CREATE TABLE `Table2Mysql` (
  `Id` char(50) NOT NULL,
  `test1` varchar(255) DEFAULT NULL,
  `test2` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of Table2Mysql
-- ----------------------------
INSERT INTO `Table2Mysql` VALUES ('bbb', 'bbb', 'bbb');

-- ----------------------------
-- Table structure for TableMysql
-- ----------------------------
DROP TABLE IF EXISTS `TableMysql`;
CREATE TABLE `TableMysql` (
  `Id` char(36) COLLATE utf8_unicode_ci NOT NULL,
  `Name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `IDNumber` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `MobilePhone` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Test1` bit(1) DEFAULT b'1',
  `Test2` int(11) DEFAULT '50',
  `Test3` text COLLATE utf8_unicode_ci,
  `CreateTime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Records of TableMysql
-- ----------------------------
INSERT INTO `TableMysql` VALUES ('02f25d22-b3d8-4e8f-9587-c8621a7ccfe5', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:17:14');
INSERT INTO `TableMysql` VALUES ('1260f496-1823-41f3-9bd1-cdfcc5eca7f2', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:17');
INSERT INTO `TableMysql` VALUES ('1557fcfc-522e-48f2-891e-0ba52dc3c8b3', '00000', 'XXXXXXX', '000', '', '50', null, '2015-08-12 13:19:54');
INSERT INTO `TableMysql` VALUES ('1584a63b-2de7-4451-bf10-850df3ac1314', '1', '2', '3', '', '50', null, '2015-08-28 10:27:50');
INSERT INTO `TableMysql` VALUES ('2d1b0339-ed0e-4827-baea-9609ba82f48a', '00000', 'XXXXXXX', '000', '', '50', null, '2015-08-12 12:27:23');
INSERT INTO `TableMysql` VALUES ('3730f083-3f1f-446a-8e17-9f938ee51de2', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:13');
INSERT INTO `TableMysql` VALUES ('37568bb2-f378-40f1-a39a-4f91e58301b6', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:06');
INSERT INTO `TableMysql` VALUES ('3f962ade-618d-45b9-9b8d-d41c27a1683d', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:10');
INSERT INTO `TableMysql` VALUES ('66fa2179-4d06-4f4b-837b-8fc3aba08aee', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:15:26');
INSERT INTO `TableMysql` VALUES ('6ef5e1da-f1ce-4ee0-a212-da9138016d1b', '777', 'XXXXXXXXXX', '7777', '', '50', null, '2015-08-12 13:18:01');
INSERT INTO `TableMysql` VALUES ('7f422300-a00d-4d53-9c9c-9310b1d97f9b', '444', '444', '4444', '', '50', null, '2015-08-12 13:17:47');
INSERT INTO `TableMysql` VALUES ('80c675c0-f451-4d92-8891-d7ce79564ca9', '00000', 'XXXXXXX', '000', '', '50', null, '2015-08-12 13:21:16');
INSERT INTO `TableMysql` VALUES ('823b69f0-9115-45d3-877b-beb6d6f8b61b', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:09');
INSERT INTO `TableMysql` VALUES ('8fda2ada-61e7-4558-ad2e-a741b60f82cb', '666', '666', '666', '', '50', null, '2015-08-12 13:17:55');
INSERT INTO `TableMysql` VALUES ('95c3d6ac-dee1-421d-90e5-3f72fca9b9aa', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:19');
INSERT INTO `TableMysql` VALUES ('bbe24fe6-d691-4d7f-87f2-3d9678c60356', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:19');
INSERT INTO `TableMysql` VALUES ('bcf6c76d-16d9-49f0-b682-12f7f7d34b7d', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:40');
INSERT INTO `TableMysql` VALUES ('cb124d27-cc1f-4120-8a25-6ea565765b25', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:17:48');
INSERT INTO `TableMysql` VALUES ('cbb54894-8736-416f-9094-213b8e1d8554', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:18');
INSERT INTO `TableMysql` VALUES ('d1cef638-988d-48d3-94fc-a90d5d61874a', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:17:59');
INSERT INTO `TableMysql` VALUES ('d6441399-810d-4208-819d-20f644739753', '111', '111', '111', '', '50', null, '2015-08-12 12:25:38');
INSERT INTO `TableMysql` VALUES ('dbae8aa7-239f-4151-8076-5789bc48de34', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:17:56');
INSERT INTO `TableMysql` VALUES ('de452398-4291-403b-8793-bab8c5c31f94', '22', '22', '222', '', '50', null, '2015-08-08 17:11:24');
INSERT INTO `TableMysql` VALUES ('e3062c28-cdfe-4dcc-bc7d-07535c651afe', 'test', 'test', 'test', '', '50', null, '2015-08-12 13:18:40');
INSERT INTO `TableMysql` VALUES ('e806a2ea-95b2-4dac-982e-483f3cc5cd92', '555', '5555', '5555', '', '50', null, '2015-08-12 13:17:53');
INSERT INTO `TableMysql` VALUES ('fdc87fad-0e80-49b2-aab0-c52d1fcd1297', '00000', '000', '000', '', '50', null, '2015-08-12 13:21:16');
