CREATE DATABASE  IF NOT EXISTS `db_shoes` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db_shoes`;
-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: db_shoes
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `manufacturers`
--

DROP TABLE IF EXISTS `manufacturers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manufacturers` (
  `manufacturer_id` int NOT NULL AUTO_INCREMENT,
  `manufacturer_name` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`manufacturer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manufacturers`
--

LOCK TABLES `manufacturers` WRITE;
/*!40000 ALTER TABLE `manufacturers` DISABLE KEYS */;
INSERT INTO `manufacturers` VALUES (1,'Alessio Nesca'),(2,'CROSBY'),(3,'Kari'),(4,'Marco Tozzi'),(5,'Rieker'),(6,'Рос');
/*!40000 ALTER TABLE `manufacturers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `order_id` int NOT NULL AUTO_INCREMENT,
  `order_date` date DEFAULT NULL,
  `order_date_delivery` date DEFAULT NULL,
  `pick_up_point_id` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `order_code` varchar(3) DEFAULT NULL,
  `order_status` varchar(8) DEFAULT NULL,
  PRIMARY KEY (`order_id`),
  KEY `fk_pick_up_point_idx` (`pick_up_point_id`),
  KEY `fk_user_id_idx` (`user_id`),
  CONSTRAINT `fk_pick_up_point` FOREIGN KEY (`pick_up_point_id`) REFERENCES `pick_up_points` (`pick_up_point_id`),
  CONSTRAINT `fk_user_id` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (2,'2022-09-28','2025-04-21',11,4,'902','Завершен'),(3,'2025-03-21','2025-04-22',2,6,'903','Завершен'),(4,'2025-02-20','2025-04-23',11,5,'904','Завершен'),(5,'2025-03-17','2025-04-24',2,10,'905','Завершен'),(6,'2025-03-01','2025-04-25',15,4,'906','Завершен'),(7,'2025-02-28','2025-04-26',3,6,'907','Завершен'),(8,'2025-03-31','2025-04-27',19,5,'908','Новый'),(9,'2025-04-02','2025-04-28',5,10,'909','Новый'),(10,'2025-04-03','2025-04-29',19,10,'910','Новый'),(12,'2026-02-24','2026-02-24',26,4,'805','Новый'),(13,'2026-03-01','2026-03-02',6,4,'950','Новый');
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pick_up_points`
--

DROP TABLE IF EXISTS `pick_up_points`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pick_up_points` (
  `pick_up_point_id` int NOT NULL AUTO_INCREMENT,
  `pick_up_point_index` varchar(6) DEFAULT NULL,
  `pick_up_point_city` varchar(35) DEFAULT NULL,
  `pick_up_point_street` varchar(45) DEFAULT NULL,
  `pick_up_point_home` varchar(5) DEFAULT NULL,
  PRIMARY KEY (`pick_up_point_id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pick_up_points`
--

LOCK TABLES `pick_up_points` WRITE;
/*!40000 ALTER TABLE `pick_up_points` DISABLE KEYS */;
INSERT INTO `pick_up_points` VALUES (1,'420151','г. Лесной','ул. Вишневая','32'),(2,'125061','г. Лесной','ул. Подгорная','8'),(3,'630370','г. Лесной','ул. Шоссейная','24'),(4,'400562','г. Лесной','ул. Зеленая','32'),(5,'614510','г. Лесной','ул. Маяковского','47'),(6,'410542','г. Лесной','ул. Светлая','46'),(7,'620839','г. Лесной','ул. Цветочная','8'),(8,'443890','г. Лесной','ул. Коммунистическая','1'),(9,'603379','г. Лесной','ул. Спортивная','46'),(10,'603721','г. Лесной','ул. Гоголя','41'),(11,'410172','г. Лесной','ул. Северная','13'),(12,'614611','г. Лесной','ул. Молодежная','50'),(13,'454311','г.Лесной','ул. Новая','19'),(14,'660007','г.Лесной','ул. Октябрьская','19'),(15,'603036','г. Лесной','ул. Садовая','4'),(16,'394060','г.Лесной','ул. Фрунзе','43'),(17,'410661','г. Лесной','ул. Школьная','50'),(18,'625590','г. Лесной','ул. Коммунистическая','20'),(19,'625683','г. Лесной','ул. 8 Марта','30'),(20,'450983','г.Лесной','ул. Комсомольская','26'),(21,'394782','г. Лесной','ул. Чехова','3'),(22,'603002','г. Лесной','ул. Дзержинского','28'),(23,'450558','г. Лесной','ул. Набережная','30'),(24,'344288','г. Лесной','ул. Чехова','1'),(25,'614164','г.Лесной','ул. Степная','30'),(26,'394242','г. Лесной','ул. Коммунистическая','43'),(27,'660540','г. Лесной','ул. Солнечная','25'),(28,'125837','г. Лесной','ул. Шоссейная','40'),(29,'125703','г. Лесной','ул. Партизанская','49'),(30,'625283','г. Лесной','ул. Победы','46'),(31,'614753','г. Лесной','ул. Полевая','35'),(32,'426030','г. Лесной','ул. Маяковского','44'),(33,'450375','г. Лесной','ул. Клубная','44'),(34,'625560','г. Лесной','ул. Некрасова','12'),(35,'630201','г. Лесной','ул. Комсомольская','17'),(36,'190949','г. Лесной','ул. Мичурина','26');
/*!40000 ALTER TABLE `pick_up_points` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `roles` (
  `role_id` int NOT NULL AUTO_INCREMENT,
  `role_name` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`role_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (1,'Авторизированный клиент'),(2,'Администратор'),(3,'Менеджер');
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `structure_orders`
--

DROP TABLE IF EXISTS `structure_orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `structure_orders` (
  `structure_order_id` int NOT NULL AUTO_INCREMENT,
  `order_id` int DEFAULT NULL,
  `tovar_article` varchar(6) DEFAULT NULL,
  `structure_order_tovar_count` int DEFAULT NULL,
  PRIMARY KEY (`structure_order_id`),
  KEY `fk_order_idx` (`order_id`),
  KEY `fk_tovar_idx` (`tovar_article`),
  CONSTRAINT `fk_order` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`),
  CONSTRAINT `fk_tovar` FOREIGN KEY (`tovar_article`) REFERENCES `tovars` (`tovar_article`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `structure_orders`
--

LOCK TABLES `structure_orders` WRITE;
/*!40000 ALTER TABLE `structure_orders` DISABLE KEYS */;
INSERT INTO `structure_orders` VALUES (2,2,'H782T5',1),(3,3,'J384T6',10),(4,4,'F572H7',5),(5,5,'А112Т4',2),(6,6,'H782T5',1),(7,7,'J384T6',10),(8,8,'F572H7',5),(9,9,'B320R5',5),(10,10,'S213E3',5),(12,2,'G783F5',1),(13,3,'D572U8',10),(14,4,'D329H3',4),(15,5,'F635R4',2),(16,6,'G783F5',1),(17,7,'D572U8',10),(18,8,'D329H3',4),(19,9,'G432E4',1),(20,10,'E482R4',5),(23,12,'D268G5',1),(24,12,'123456',5),(25,13,'C436G5',1);
/*!40000 ALTER TABLE `structure_orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `supplier_name` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (1,'Kari'),(2,'Обувь для вас');
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tovar_categories`
--

DROP TABLE IF EXISTS `tovar_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tovar_categories` (
  `tovar_category_id` int NOT NULL AUTO_INCREMENT,
  `tovar_category_name` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`tovar_category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovar_categories`
--

LOCK TABLES `tovar_categories` WRITE;
/*!40000 ALTER TABLE `tovar_categories` DISABLE KEYS */;
INSERT INTO `tovar_categories` VALUES (1,'Женская обувь'),(2,'Мужская обувь');
/*!40000 ALTER TABLE `tovar_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tovars`
--

DROP TABLE IF EXISTS `tovars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tovars` (
  `tovar_article` varchar(6) NOT NULL,
  `tovar_name` varchar(45) DEFAULT NULL,
  `tovar_unit` varchar(5) DEFAULT NULL,
  `tovar_cost` decimal(10,2) DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `manufacturer_id` int DEFAULT NULL,
  `tovar_category_id` int DEFAULT NULL,
  `tovar_discount` int DEFAULT NULL,
  `tovar_count` int DEFAULT NULL,
  `tovar_description` text,
  `tovar_image` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`tovar_article`),
  KEY `fk_supplier_idx` (`supplier_id`),
  KEY `fk_manufacturer_idx` (`manufacturer_id`),
  KEY `fk_tovar_category_idx` (`tovar_category_id`),
  CONSTRAINT `fk_manufacturer` FOREIGN KEY (`manufacturer_id`) REFERENCES `manufacturers` (`manufacturer_id`),
  CONSTRAINT `fk_supplier` FOREIGN KEY (`supplier_id`) REFERENCES `suppliers` (`supplier_id`),
  CONSTRAINT `fk_tovar_category` FOREIGN KEY (`tovar_category_id`) REFERENCES `tovar_categories` (`tovar_category_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovars`
--

LOCK TABLES `tovars` WRITE;
/*!40000 ALTER TABLE `tovars` DISABLE KEYS */;
INSERT INTO `tovars` VALUES ('123456','3к','шт.',2344.00,2,2,1,5,0,'23к','123456_1.jpg'),('9RC4QJ','asd','шт.',1000.00,2,4,2,0,2,'zxc','9RC4QJ.png'),('B320R5','Туфли','шт.',4300.00,1,5,1,2,6,'Туфли Rieker женские демисезонные, размер 41, цвет коричневый','-'),('C436G5','Ботинки','шт.',10200.00,1,1,1,15,8,'Ботинки женские, ARGO, размер 40','-'),('D268G5','Туфли','шт.',4399.00,2,5,1,3,10,'Туфли Rieker женские демисезонные, размер 36, цвет коричневый','-'),('D329H3','Полуботинки','шт.',1890.00,2,1,1,4,4,'Полуботинки Alessio Nesca женские 3-30797-47, размер 37, цвет: бордовый','8.jpg'),('D364R4','Туфли','шт.',12400.00,1,3,1,16,5,'Туфли Luiza Belly женские Kate-lazo черные из натуральной замши','-'),('D572U8','Кроссовки','шт.',4100.00,2,6,2,3,6,'129615-4 Кроссовки мужские','6.jpg'),('E482R4','Полуботинки','шт.',1800.00,1,3,1,2,13,'Полуботинки kari женские MYZ20S-149, размер 41, цвет: черный','-'),('F427R5','Ботинки','шт.',11800.00,2,5,1,15,11,'Ботинки на молнии с декоративной пряжкой FRAU','-'),('F572H7','Туфли','шт.',2700.00,1,4,1,2,14,'Туфли Marco Tozzi женские летние, размер 39, цвет черный','7.jpg'),('F635R4','Ботинки','шт.',3244.00,2,4,1,2,13,'Ботинки Marco Tozzi женские демисезонные, размер 39, цвет бежевый','2.jpg'),('G432E4','Туфли','шт.',2800.00,1,3,1,3,15,'Туфли kari женские TR-YR-413017, размер 37, цвет: черный','10.jpg'),('G531F4','Ботинки','шт.',6600.00,1,3,1,12,9,'Ботинки женские зимние ROMER арт. 893167-01 Черный','-'),('G783F5','Ботинки','шт.',5900.00,1,6,2,2,8,'Мужские ботинки Рос-Обувь кожаные с натуральным мехом','4.jpg'),('H535R5','Ботинки','шт.',2300.00,2,5,1,2,7,'Женские Ботинки демисезонные','-'),('H782T5','Туфли','шт.',4499.00,1,3,2,4,5,'Туфли kari мужские классика MYZ21AW-450A, размер 43, цвет: черный','3.jpg'),('J384T6','Ботинки','шт.',3800.00,2,5,2,2,16,'B3430/14 Полуботинки мужские Rieker','5.jpg'),('J542F5','Тапочки','шт.',500.00,1,3,2,13,0,'Тапочки мужские Арт.70701-55-67син р.41','-'),('K345R4','Полуботинки','шт.',2100.00,2,2,2,2,3,'407700/01-02 Полуботинки мужские CROSBY','-'),('K358H6','Тапочки','шт.',599.00,1,5,2,20,2,'Тапочки мужские син р.41','-'),('L754R4','Полуботинки','шт.',1700.00,1,3,1,2,7,'Полуботинки kari женские WB2020SS-26, размер 38, цвет: черный','-'),('M542T5','Кроссовки','шт.',2800.00,2,5,2,18,3,'Кроссовки мужские TOFA','-'),('N457T5','Полуботинки','шт.',4600.00,1,2,1,3,13,'Полуботинки Ботинки черные зимние, мех','-'),('O754F4','Туфли','шт.',5400.00,2,5,1,4,18,'Туфли женские демисезонные Rieker артикул 55073-68/37','-'),('P764G4','Туфли','шт.',6800.00,1,2,1,15,15,'Туфли женские, ARGO, размер 38','-'),('P9TH7C','апрол','шт.',567.00,1,1,1,99,0,'прол','picture.png'),('S213E3','Полуботинки','шт.',2156.00,2,2,2,3,6,'407700/01-01 Полуботинки мужские CROSBY','-'),('S634B5','Кеды','шт.',5500.00,2,2,2,3,0,'Кеды Caprice мужские демисезонные, размер 42, цвет черный','S634B5.png'),('А112Т4','Ботинки','шт.',4990.00,1,3,1,3,6,'Женские Ботинки демисезонные kari','1.jpg');
/*!40000 ALTER TABLE `tovars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `role_id` int DEFAULT NULL,
  `user_surname` varchar(25) DEFAULT NULL,
  `user_name` varchar(20) DEFAULT NULL,
  `user_lastname` varchar(25) DEFAULT NULL,
  `user_login` varchar(45) DEFAULT NULL,
  `user_pass` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`user_id`),
  KEY `fk_role_idx` (`role_id`),
  CONSTRAINT `fk_role` FOREIGN KEY (`role_id`) REFERENCES `roles` (`role_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,3,'Ворсин','Петр','Евгеньевич','tjde7c@yahoo.com','YOyhfR'),(2,1,'Ворсин','Петр','Евгеньевич','1qz4kw@mail.com','gynQMT'),(3,1,'Михайлюк','Анна','Вячеславовна','5d4zbu@tutanota.com','rwVDh9'),(4,2,'Никифорова','Весения','Николаевна','94d5ous@gmail.com','uzWC67'),(5,2,'Одинцов','Серафим','Артёмович','yzls62@outlook.com','JlFRCZ'),(6,2,'Сазонов','Руслан','Германович','uth4iz@mail.com','2L6KZG'),(7,1,'Ситдикова','Елена','Анатольевна','ptec8ym@yahoo.com','LdNyos'),(8,3,'Старикова','Елена','Павловна','wpmrc3do@tutanota.com','RSbvHv'),(9,1,'Старикова','Елена','Павловна','4np6se@mail.com','AtnDjr'),(10,3,'Степанов','Михаил','Артёмович','1diph5e@tutanota.com','8ntwUp');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-11  9:39:10
