/*
Navicat MySQL Data Transfer

Source Server         : 127.0.0.1_3306
Source Server Version : 50528
Source Host           : 127.0.0.1:3306
Source Database       : DosORMTest

Target Server Type    : MYSQL
Target Server Version : 50528
File Encoding         : 65001

Date: 2015-08-03 14:01:10
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for TestTable
-- ----------------------------
DROP TABLE IF EXISTS `TestTable`;
CREATE TABLE `TestTable` (
  `Id` char(36) COLLATE utf8_unicode_ci NOT NULL,
  `Name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `IDNumber` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `MobilePhone` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Test1` bit(1) DEFAULT b'1',
  `Test2` int(11) DEFAULT '50',
  `CreateTime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Records of TestTable
-- ----------------------------
INSERT INTO `TestTable` VALUES ('1291a1b7-51e5-4078-af63-c43cb6dca184', '999', '9999', '999', '', '50', '2015-08-03 13:46:52');
INSERT INTO `TestTable` VALUES ('16db5f46-98d0-4384-a655-ec7bcca5fad1', '4444', '4444', '44444', '', '50', '2015-08-03 13:20:50');
INSERT INTO `TestTable` VALUES ('7e0c9953-b7e4-440d-970d-764eac080c18', '77777', '7777', '7777', '', '50', '2015-08-03 13:30:28');
INSERT INTO `TestTable` VALUES ('a6bf3009-a57a-4d4a-a064-ef43d66ca8ec', '88888~', '888~', '88888', '', '50', '2015-08-03 13:40:01');
INSERT INTO `TestTable` VALUES ('a7654b1b-8163-4268-b96e-9e36bff10110', '11111', '11111', '1111111', '', '50', '2015-08-03 13:21:12');
INSERT INTO `TestTable` VALUES ('bbf50f8a-d50a-45a3-8b21-5b1b47d4a3dd', '222', '222', '2222', '', '50', '2015-08-03 13:20:43');
INSERT INTO `TestTable` VALUES ('ca204919-73dd-4d13-9aa9-f3a84c75ff07', '6666', '6666', '666666', '', '50', '2015-08-03 13:30:24');
INSERT INTO `TestTable` VALUES ('d072d705-40b0-4c46-ba39-415cf1670d35', '5555', '555', '555', '', '50', '2015-08-03 13:30:20');
INSERT INTO `TestTable` VALUES ('dc082ae1-13b8-4c51-ab58-ab7d8433082a', '3333', '333', '3333', '', '50', '2015-08-03 13:20:46');
