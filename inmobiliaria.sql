-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: inmobiliaria
-- ------------------------------------------------------
-- Server version	8.0.42

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
-- Table structure for table `contratos`
--

DROP TABLE IF EXISTS `contratos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contratos` (
  `IdContrato` int NOT NULL AUTO_INCREMENT,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `Monto` decimal(18,2) NOT NULL,
  `InmuebleId` int NOT NULL,
  `InquilinoId` int NOT NULL,
  PRIMARY KEY (`IdContrato`),
  KEY `InmuebleId` (`InmuebleId`),
  KEY `InquilinoId` (`InquilinoId`),
  CONSTRAINT `contratos_ibfk_1` FOREIGN KEY (`InmuebleId`) REFERENCES `inmueble` (`id_inmueble`),
  CONSTRAINT `contratos_ibfk_2` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilino` (`IdInquilino`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contratos`
--

LOCK TABLES `contratos` WRITE;
/*!40000 ALTER TABLE `contratos` DISABLE KEYS */;
INSERT INTO `contratos` VALUES (1,'2025-09-03','2025-09-10',200000.00,1,4),(2,'2025-09-03','2025-09-10',200000.00,2,5);
/*!40000 ALTER TABLE `contratos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inmueble`
--

DROP TABLE IF EXISTS `inmueble`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inmueble` (
  `id_inmueble` int NOT NULL AUTO_INCREMENT,
  `direccion` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `tipo_inmueble` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `estado` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `ambientes` int NOT NULL,
  `superficie` int NOT NULL,
  `precio` decimal(10,2) NOT NULL,
  `propietario_id` int NOT NULL,
  PRIMARY KEY (`id_inmueble`),
  KEY `propietario_id` (`propietario_id`),
  CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`propietario_id`) REFERENCES `propietario` (`id_propietario`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inmueble`
--

LOCK TABLES `inmueble` WRITE;
/*!40000 ALTER TABLE `inmueble` DISABLE KEYS */;
INSERT INTO `inmueble` VALUES (1,'junin 1234','Departamento','Libre',3,500,1000000.00,1),(2,'colon 4567','Casa','Disponible',4,1499,253000.00,1),(3,'colon 4567','Departamento','Disponible',4,500,456000.00,1);
/*!40000 ALTER TABLE `inmueble` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inquilino`
--

DROP TABLE IF EXISTS `inquilino`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inquilino` (
  `IdInquilino` int NOT NULL AUTO_INCREMENT,
  `Documento` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Apellido` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Nombre` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Email` varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Telefono` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Direccion` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Baja` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`IdInquilino`),
  UNIQUE KEY `uq_inquilino_documento` (`Documento`),
  KEY `idx_inquilino_apellido` (`Apellido`,`Nombre`),
  KEY `idx_inquilino_doc` (`Documento`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inquilino`
--

LOCK TABLES `inquilino` WRITE;
/*!40000 ALTER TABLE `inquilino` DISABLE KEYS */;
INSERT INTO `inquilino` VALUES (4,'30567789','Suarez','Tomasa','tomasa@gmail.com','2665677890','junin 1234',0),(5,'44643275','Saez','Ernesto','ernestoejemplo@gmail.com','2665467894','aristobulo del valle 1216',0);
/*!40000 ALTER TABLE `inquilino` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propietario`
--

DROP TABLE IF EXISTS `propietario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propietario` (
  `id_propietario` int NOT NULL AUTO_INCREMENT,
  `documento` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `apellido` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `telefono` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `direccion` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `baja` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id_propietario`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propietario`
--

LOCK TABLES `propietario` WRITE;
/*!40000 ALTER TABLE `propietario` DISABLE KEYS */;
INSERT INTO `propietario` VALUES (1,'34456789','Perez','Cesar','cesarperez@gmail.com','266437899','tavarez 1234',0);
/*!40000 ALTER TABLE `propietario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-09-04 12:55:04
