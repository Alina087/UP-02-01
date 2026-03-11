CREATE DATABASE  IF NOT EXISTS `db_cosmetic` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db_cosmetic`;
-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: db_cosmetic
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
-- Table structure for table `carts`
--

DROP TABLE IF EXISTS `carts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `carts` (
  `user_id` int NOT NULL,
  `tovar_article` varchar(6) NOT NULL,
  `cart_tovar_count` int DEFAULT NULL,
  PRIMARY KEY (`user_id`,`tovar_article`),
  KEY `fk_tovar_idx` (`tovar_article`),
  CONSTRAINT `fk_tovars` FOREIGN KEY (`tovar_article`) REFERENCES `tovars` (`tovar_article`),
  CONSTRAINT `fk_users` FOREIGN KEY (`user_id`) REFERENCES `users` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `carts`
--

LOCK TABLES `carts` WRITE;
/*!40000 ALTER TABLE `carts` DISABLE KEYS */;
/*!40000 ALTER TABLE `carts` ENABLE KEYS */;
UNLOCK TABLES;

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
INSERT INTO `manufacturers` VALUES (1,'L\'Oréal'),(2,'Maybelline'),(3,'Estée Lauder'),(4,'NYX'),(5,'Clinique'),(6,'Catrice');
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
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,'2025-02-27','2025-04-20',1,10,'901','Завершен'),(2,'2022-09-28','2025-04-21',11,4,'902','Завершен'),(3,'2025-03-21','2025-04-22',2,6,'903','Завершен'),(4,'2025-02-20','2025-04-23',11,5,'904','Завершен'),(5,'2025-03-17','2025-04-24',2,10,'905','Завершен'),(6,'2025-03-01','2025-04-25',15,4,'906','Завершен'),(7,'2025-02-28','2025-04-26',3,6,'907','Завершен'),(8,'2025-03-31','2025-04-27',19,5,'908','Новый'),(9,'2025-04-02','2025-04-28',5,10,'909','Новый'),(10,'2025-04-03','2026-02-23',19,10,'910','Завершен'),(11,'2026-02-23','2026-02-23',1,12,'570','Завершен'),(12,'2026-02-23','2026-02-23',1,12,'749','Завершен'),(13,'2026-02-23',NULL,2,12,'976','Новый'),(14,'2026-02-24',NULL,2,12,'153','Новый');
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
INSERT INTO `pick_up_points` VALUES (1,'420151','г. Уфа','ул. Вишневая','32'),(2,'125061','г. Уфа','ул. Подгорная','8'),(3,'630370','г. Уфа','ул. Шоссейная','24'),(4,'400562','г. Уфа','ул. Зеленая','32'),(5,'614510','г. Уфа','ул. Маяковского','47'),(6,'410542','г. Уфа','ул. Светлая','46'),(7,'620839','г. Уфа','ул. Цветочная','8'),(8,'443890','г. Уфа','ул. Коммунистическая','1'),(9,'603379','г. Уфа','ул. Спортивная','46'),(10,'603721','г. Уфа','ул. Гоголя','41'),(11,'410172','г. Уфа','ул. Северная','13'),(12,'614611','г. Уфа','ул. Молодежная','50'),(13,'454311','г.Уфа','ул. Новая','19'),(14,'660007','г.Уфа','ул. Октябрьская','19'),(15,'603036','г. Уфа','ул. Садовая','4'),(16,'394060','г.Уфа','ул. Фрунзе','43'),(17,'410661','г. Уфа','ул. Школьная','50'),(18,'625590','г. Уфа','ул. Коммунистическая','20'),(19,'625683','г. Уфа','ул. 8 Марта','30'),(20,'450983','г.Уфа','ул. Комсомольская','26'),(21,'394782','г. Уфа','ул. Чехова','3'),(22,'603002','г. Уфа','ул. Дзержинского','28'),(23,'450558','г. Уфа','ул. Набережная','30'),(24,'344288','г. Уфа','ул. Чехова','1'),(25,'614164','г.Уфа','ул. Степная','30'),(26,'394242','г. Уфа','ул. Коммунистическая','43'),(27,'660540','г. Уфа','ул. Солнечная','25'),(28,'125837','г. Уфа','ул. Шоссейная','40'),(29,'125703','г. Уфа','ул. Партизанская','49'),(30,'625283','г. Уфа','ул. Победы','46'),(31,'614753','г. Уфа','ул. Полевая','35'),(32,'426030','г. Уфа','ул. Маяковского','44'),(33,'450375','г. Уфа','ул. Клубная','44'),(34,'625560','г. Уфа','ул. Некрасова','12'),(35,'630201','г. Уфа','ул. Комсомольская','17'),(36,'190949','г. Уфа','ул. Мичурина','26');
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
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `structure_orders`
--

LOCK TABLES `structure_orders` WRITE;
/*!40000 ALTER TABLE `structure_orders` DISABLE KEYS */;
INSERT INTO `structure_orders` VALUES (1,1,'А112Т4',2),(2,2,'H782T5',1),(3,3,'J384T6',10),(4,4,'F572H7',5),(5,5,'А112Т4',2),(6,6,'H782T5',1),(7,7,'J384T6',10),(8,8,'F572H7',5),(9,9,'B320R5',5),(10,10,'S213E3',5),(11,1,'F635R4',2),(12,2,'G783F5',1),(13,3,'D572U8',10),(14,4,'D329H3',4),(15,5,'F635R4',2),(16,6,'G783F5',1),(17,7,'D572U8',10),(18,8,'D329H3',4),(19,9,'G432E4',1),(20,10,'E482R4',5),(21,11,'B320R5',2),(22,11,'B431R5',1),(23,12,'D364R4',1),(24,13,'F572H7',1),(25,14,'123456',1),(26,14,'135890',1);
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
INSERT INTO `suppliers` VALUES (1,'Beauty Supply Co'),(2,'Glamour Wholesale');
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
  `tovar_category_name` varchar(25) DEFAULT NULL,
  PRIMARY KEY (`tovar_category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovar_categories`
--

LOCK TABLES `tovar_categories` WRITE;
/*!40000 ALTER TABLE `tovar_categories` DISABLE KEYS */;
INSERT INTO `tovar_categories` VALUES (1,'Уходовая косметика'),(2,'Декоративная косметиа');
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
INSERT INTO `tovars` VALUES ('123456','иро','шт.',1000.00,1,6,2,10,0,'по','123456_1771903669019_yl3w63.png'),('135890','вао','шт.',10.00,1,6,2,1,0,'ро','135890_1771905418560_9ip04a.png'),('Asdf78','asdf','шт.',10.00,1,6,2,1,1,'fghj','Asdf78_1771905197687_4wtnb1.png'),('asdfgh','вао','шт.',10.00,1,6,2,1,1,'ро','135890_1771905418560_9ip04a.png'),('B320R5','Тени для век','шт.',460.00,1,5,2,2,4,'Палетка теней с матовыми и перламутровыми оттенками для создания разных образов','1.webp'),('B431R5','Крем-гель для кожи','шт.',450.00,2,5,1,2,4,'Обновляющий крем-гель с легкой текстурой для увлажнения и защиты кожи','8.jpg'),('C436G5','Скраб для лица','шт.',1150.00,1,1,1,15,9,'Мягкий скраб с натуральными абразивными частицами для очищения и обновления кожи',''),('D268G5','Пудра компактная','шт.',560.00,2,5,2,3,12,'Легкая пудра для матирования и выравнивания тона кожи',''),('D329H3','Крем для рук','шт.',210.00,2,1,1,4,4,'Питательный крем для рук с увлажняющим эффектом, быстро впитывается','6.jpg'),('D364R4','Помада матовая','шт.',670.00,1,3,2,16,4,'Интенсивная матовая помада с высокой стойкостью в яркой палитре','12.jpg'),('D572U8','Тональный крем с SPF','шт.',560.00,2,6,2,3,6,'Тональный крем с защитой SPF 30, насыщенный и увлажняющий',''),('dfgh22','ghj','шт.',10.00,1,3,1,10,10,'fhyjk','dfgh22_1771906804235_ot7nuq.png'),('E482R4','Лосьон тонизирующий','шт.',250.00,1,3,1,2,14,'Освежающий тоник с экстрактами растений, увлажняет и успокаивает кожу','9.jpg'),('F427R5','Тональный крем стойкий','шт.',740.00,2,5,2,15,11,'Длительно держится, обеспечивает ровное покрытие и контроль блеска',''),('F572H7','Блеск для губ','шт.',340.00,1,4,2,2,13,'Увлажняющий блеск с лёгким оттенком и сиянием для натурального эффекта','2.jpg'),('F635R4','Тональный крем','шт.',450.00,2,4,2,2,13,'Лёгкий тональный крем средней плотности, выравнивает тон лица, придает естественное сияние','F635R4_1770723120525_771.jpg'),('G432E4','Румяна компактные','шт.',350.00,1,3,2,3,15,'Компактные румяна с нежным оттенком, придают естественное сияние щекам',''),('G531F4','Маска для лица','шт.',890.00,1,3,1,12,9,'Увлажняющая маска с экстрактом алоэ вера для глубокого питания кожи','10.webp'),('G783F5','Сыворотка для лица','шт.',780.00,1,6,1,2,8,'Активная сыворотка с витаминами для глубокого питания и защиты кожи',''),('H535R5','Крем для лица питательный','шт.',520.00,2,5,1,2,7,'Питательный крем с комплексом витаминов для ежедневного ухода',''),('H782T5','Тушь для ресниц','шт.',520.00,1,3,2,4,5,'Удлиняющая и создающая объём тушь, устойчивая к влаге и осыпанию',''),('J384T6','Помада стойкая','шт.',420.00,2,5,2,2,16,'Матовая стойкая помада с бархатистым финишем и насыщенным цветом',''),('J542F5','Помада для губ питательная','шт.',220.00,1,3,2,13,0,'Помада с увлажняющими маслами, обеспечивает стойкий комфорт губам','11.jpg'),('K345R4','Консилер светлый','шт.',380.00,2,2,2,2,3,'Консилер для маскировки несовершенств и темных кругов под глазами','4.webp'),('K358H6','Бальзам для губ','шт.',210.00,1,5,1,20,2,'Увлажняющий бальзам для защиты и смягчения губ','1.jpg'),('L754R4','Тоник для лица','шт.',350.00,1,3,1,2,7,'Освежающий тоник для снятия загрязнений и увлажнения кожи',''),('M542T5','Гель для умывания','шт.',420.00,2,5,1,18,3,'Мягкий гель для умывания с успокаивающим эффектом, подходит для чувствительной кожи','3.jpg'),('N457T5','Крем защитный для лица','шт.',590.00,1,2,1,3,13,'Защитный крем против воздействия окружающей среды с антиоксидантами','N457T5_1770724106412_zsj7zm.jpg'),('O754F4','Крем для лица дневной','шт.',710.00,2,5,1,4,18,'Легкий дневной крем c защитой от УФ и увлажняющими компонентами','5.jpg'),('P764G4','Тушь объемная','шт.',690.00,1,2,2,15,15,'Тушь для создания максимального объёма ресниц с эффектом накладных ресниц',''),('S213E3','Крем для лица ночной','шт.',680.00,2,2,1,3,6,'Ночной крем с регенерирующими компонентами, интенсивное восстановление кожи','S213E3_1770726558109_q3edue.jpg'),('S326R5','Крем для ног','шт.',530.00,2,2,1,17,15,'Питательный крем с охлаждающим эффектом и натуральными маслами',''),('S634B5','Пудра рассыпчатая','шт.',540.00,2,2,2,3,0,'Лёгкая пудра для фиксации макияжа с матирующим эффектом',''),('T324F5','Крем для кожи вокруг глаз','шт.',480.00,1,2,1,2,5,'Увлажняющий и питающий крем для деликатной зоны вокруг глаз','T324F5_1770725920262_ucgju0.jpg'),('zxcvbn','fghjk','шт.',110.00,1,6,2,1,1,'fghjk','zxcvbn_1771905732972_ug51h1.png'),('А112Т4','Крем увлажняющий','шт.',590.00,1,3,1,3,6,'Увлажняющий крем для лица с насыщенной текстурой, подходит для всех типов кожи','7.jpg');
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
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,3,'Ворсин','Петр','Евгеньевич','tjde7c@yahoo.com','YOyhfR'),(2,1,'Ворсин','Петр','Евгеньевич','1qz4kw@mail.com','gynQMT'),(3,1,'Михайлюк','Анна','Вячеславовна','5d4zbu@tutanota.com','rwVDh9'),(4,2,'Никифорова','Весения','Николаевна','94d5ous@gmail.com','uzWC67'),(5,2,'Одинцов','Серафим','Артёмович','yzls62@outlook.com','JlFRCZ'),(6,2,'Сазонов','Руслан','Германович','uth4iz@mail.com','2L6KZG'),(7,1,'Ситдикова','Елена','Анатольевна','ptec8ym@yahoo.com','LdNyos'),(8,3,'Старикова','Елена','Павловна','wpmrc3do@tutanota.com','RSbvHv'),(9,1,'Старикова','Елена','Павловна','4np6se@mail.com','AtnDjr'),(10,3,'Степанов','Михаил','Артёмович','1diph5e@tutanota.com','8ntwUp'),(12,1,'Кильдибаева','Алина','Альмировна','alina@gmail.com','12345678');
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

-- Dump completed on 2026-03-11  9:40:15
