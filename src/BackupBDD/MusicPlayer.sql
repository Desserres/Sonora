-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Host: mysql
-- Generation Time: May 20, 2026 at 08:38 AM
-- Server version: 8.0.46
-- PHP Version: 8.3.26

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `MusicPlayer`
--

-- --------------------------------------------------------

--
-- Table structure for table `MusicPlayer`
--

CREATE TABLE `MusicPlayer` (
  `id` int NOT NULL,
  `title` text NOT NULL,
  `image` text NOT NULL,
  `file` text NOT NULL,
  `est_favori` tinyint(1) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `MusicPlayer`
--

INSERT INTO `MusicPlayer` (`id`, `title`, `image`, `file`, `est_favori`) VALUES
(9, 'Jul - La faille', '/images/jul.png', '/music/9_jul___la_faille.mp3', 0),
(10, 'Jul - J\'oublie tout', '/images/jul.png', '/music/10_jul___joublie_tout.mp3', 1),
(11, 'Jul - Sous la lune', '/images/jul.png', '/music/11_jul___sous_la_lune.mp3', 0),
(14, 'Ninho - Maman ne le sait pas', '/images/ninho.png', '/music/14_ninho___maman_ne_le_sait_pas.mp3', 0),
(15, 'SCH - Autobahn', '/images/sch.png', '/music/15_sch___autobahn.mp3', 0),
(16, 'SCH - Marché noir', '/images/sch.png', '/music/16_sch___marche_noir.mp3', 1),
(17, 'SCH - Champs-Élysées', '/images/sch.png', '/music/17_sch___champs_elysees.mp3', 1),
(18, 'Gazo - Drill FR 4', '/images/gazo.png', '/music/18_gazo___drill_fr_4.mp3', 1),
(19, 'Gazo - Haine&Sex', '/images/gazo.png', '/music/19_gazo___haine_sex.mp3', 0),
(20, 'Gazo - RAPPEL', '/images/gazo.png', '/music/20_gazo___rappel.mp3', 0),
(21, 'Damso - Macarena', '/images/damso.png', '/music/21_damso___macarena.mp3', 0),
(22, 'Damso - Smog', '/images/damso.png', '/music/22_damso___smog.mp3', 0),
(23, 'Damso - Nwaar Is the New Black', '/images/damso.png', '/music/23_damso___nwaar_is_the_new_black.mp3', 0),
(24, 'PLK - Problèmes', '/images/plk.png', '/music/24_plk___problemes.mp3', 0),
(25, 'PLK - Train de vie', '/images/plk.png', '/music/25_plk___train_de_vie.mp3', 0),
(26, 'PLK - On s\'comprend', '/images/plk.png', '/music/26_plk___on_scomprend.mp3', 0),
(27, 'Booba - Dolce Vita', '/images/booba.png', '/music/27_booba___dolce_vita.mp3', 0),
(28, 'Booba - Ratpi World', '/images/booba.png', '/music/28_booba___ratpi_world.mp3', 0),
(29, 'Booba - Validée', '/images/booba.png', '/music/29_booba___validee.mp3', 0),
(30, 'Nekfeu - On verra', '/images/nekfeu.png', '/music/30_nekfeu___on_verra.mp3', 0),
(31, 'Nekfeu - Humanoïde', '/images/nekfeu.png', '/music/31_nekfeu___humanoide.mp3', 1),
(32, 'Nekfeu - Tempête', '/images/nekfeu.png', '/music/32_nekfeu___tempete.mp3', 0),
(33, 'Leto - Train de vie', '/images/leto.png', '/music/33_leto___train_de_vie.mp3', 0),
(34, 'Leto - Tout recommencer', '/images/leto.png', '/music/34_leto___tout_recommencer.mp3', 0),
(36, 'Tiakola - Gasolina', '/images/tiakola.png', '/music/36_tiakola___gasolina.mp3', 0),
(37, 'Tiakola - Meuda', '/images/tiakola.png', '/music/37_tiakola___meuda.mp3', 0),
(38, 'Tiakola - La mélo est gangx', '/images/tiakola.png', '/music/38_tiakola___la_melo_est_gangx.mp3', 0),
(39, 'Jul - Bande organisée', '/images/jul.png', '/music/39_jul___bande_organisee.mp3', 0),
(40, 'Jul - Wesh alors', '/images/jul.png', '/music/40_jul___wesh_alors.mp3', 0),
(41, 'Jul - J\'fais mes affaires', '/images/jul.png', '/music/41_jul___jfais_mes_affaires.mp3', 0),
(42, 'Jul - Ma jolie', '/images/jul.png', '/music/42_jul___ma_jolie.mp3', 0),
(43, 'Ninho - Jefe', '/images/ninho.png', '/music/43_ninho___jefe.mp3', 0),
(44, 'Ninho - Zipette', '/images/ninho.png', '/music/44_ninho___zipette.mp3', 0),
(45, 'Ninho - Goutte d\'eau', '/images/ninho.png', '/music/45_ninho___goutte_deau.mp3', 0),
(46, 'Ninho - Air Max', '/images/ninho.png', '/music/46_ninho___air_max.mp3', 0),
(47, 'SCH - R.A.C', '/images/sch.png', '/music/47_sch___rac.mp3', 0),
(48, 'SCH - Gibraltar', '/images/sch.png', '/music/48_sch___gibraltar.mp3', 0),
(49, 'SCH - Champs-Élysées', '/images/sch.png', '/music/49_sch___champs_elysees.mp3', 1),
(50, 'Gazo - CASANOVA', '/images/gazo.png', '/music/50_gazo___casanova.mp3', 0),
(51, 'Gazo - RAPPEL', '/images/gazo.png', '/music/51_gazo___rappel.mp3', 0),
(52, 'Gazo - DIE', '/images/gazo.png', '/music/52_gazo___die.mp3', 0),
(54, 'Damso - Amnésie', '/images/damso.png', '/music/54_damso___amnesie.mp3', 0),
(55, 'Damso - Macarena', '/images/damso.png', '/music/55_damso___macarena.mp3', 0),
(56, 'PLK - Pilote', '/images/plk.png', '/music/56_plk___pilote.mp3', 0),
(57, 'PLK - Dis-moi oui', '/images/plk.png', '/music/57_plk___dis_moi_oui.mp3', 0),
(59, 'Booba - DKR', '/images/booba.png', '/music/59_booba___dkr.mp3', 0),
(60, 'Booba - 92i Veyron', '/images/booba.png', '/music/60_booba___92i_veyron.mp3', 0),
(61, 'Booba - Scarface', '/images/booba.png', '/music/61_booba___scarface.mp3', 0),
(62, 'Nekfeu - On verra', '/images/nekfeu.png', '/music/62_nekfeu___on_verra.mp3', 0),
(63, 'Nekfeu - Égérie', '/images/nekfeu.png', '/music/63_nekfeu___egerie.mp3', 0),
(66, 'Leto - Paris c\'est magique', '/images/leto.png', '/music/66_leto___paris_cest_magique.mp3', 0),
(67, 'Tiakola - Meuda', '/images/tiakola.png', '/music/67_tiakola___meuda.mp3', 0),
(71, 'Egg', '/music/11_jul___sous_la_lune.mp3', '', 3);

-- --------------------------------------------------------

--
-- Table structure for table `Users`
--

CREATE TABLE `Users` (
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `id` int NOT NULL,
  `email` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `Users`
--

INSERT INTO `Users` (`username`, `password`, `id`, `email`) VALUES
('baptiste', '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08', 16, 'baptiste123.0000@gmail.com');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `MusicPlayer`
--
ALTER TABLE `MusicPlayer`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `MusicPlayer`
--
ALTER TABLE `MusicPlayer`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=72;

--
-- AUTO_INCREMENT for table `Users`
--
ALTER TABLE `Users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
