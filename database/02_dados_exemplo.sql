USE retrogames;

-- Tabelas de domínio (chaves fixas)
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
(1, 'Novo'),
(2, 'Usado'),
(3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
(1, 'Administrador'),
(2, 'Cliente');

-- Plataformas (ids 1 a 6, num banco recém-criado)
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
('Nintendo Entertainment System', 'Nintendo', 1983),
('Super Nintendo', 'Nintendo', 1990),
('Mega Drive', 'Sega', 1988),
('PlayStation', 'Sony', 1994),
('Nintendo 64', 'Nintendo', 1996),
('Game Boy', 'Nintendo', 1989);

-- Gêneros (ids 1 a 8)
INSERT INTO tb_genero (no_genero) VALUES
('Plataforma'),   -- 1
('Aventura'),     -- 2
('RPG'),          -- 3
('Corrida'),      -- 4
('Luta'),         -- 5
('Ação'),         -- 6
('Esporte'),      -- 7
('Puzzle');       -- 8

-- Jogos (condição: 1 Novo, 2 Usado, 3 Recondicionado)
-- Colunas: nome, descrição, ano, preço, estoque, ativo, cadastro, plataforma, condição
INSERT INTO tb_jogo (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES
('Super Mario Bros.', 'Clássico de plataforma do NES.', 1985, 149.90, 3, TRUE, UTC_TIMESTAMP(), 1, 2),             -- 1
('The Legend of Zelda', 'Aventura em Hyrule.', 1986, 189.90, 2, TRUE, UTC_TIMESTAMP(), 1, 2),                      -- 2
('Mega Man 2', 'Ação e plataforma com chefes marcantes.', 1988, 169.90, 1, TRUE, UTC_TIMESTAMP(), 1, 3),           -- 3
('Super Mario World', 'Mario e Yoshi no SNES.', 1990, 229.90, 4, TRUE, UTC_TIMESTAMP(), 2, 2),                     -- 4
('Chrono Trigger', 'RPG com viagem no tempo.', 1995, 399.90, 1, TRUE, UTC_TIMESTAMP(), 2, 3),                      -- 5
('Street Fighter II Turbo', 'Jogo de luta clássico.', 1993, 179.90, 2, TRUE, UTC_TIMESTAMP(), 2, 2),               -- 6
('F-Zero', 'Corrida futurista em alta velocidade.', 1990, 129.90, 0, TRUE, UTC_TIMESTAMP(), 2, 2),                 -- 7 (estoque zero)
('Sonic the Hedgehog', 'O ouriço mais rápido dos games.', 1991, 139.90, 5, TRUE, UTC_TIMESTAMP(), 3, 2),          -- 8
('Streets of Rage 2', 'Beat em up de ação.', 1992, 159.90, 2, TRUE, UTC_TIMESTAMP(), 3, 2),                        -- 9
('Phantasy Star IV', 'RPG de ficção científica.', 1993, 349.90, 1, TRUE, UTC_TIMESTAMP(), 3, 3),                   -- 10
('Mortal Kombat II', 'Luta brutal e icônica.', 1993, 119.90, 3, TRUE, UTC_TIMESTAMP(), 3, 2),                      -- 11
('Gran Turismo', 'Simulador de corrida.', 1997, 99.90, 4, TRUE, UTC_TIMESTAMP(), 4, 2),                            -- 12
('Final Fantasy VII', 'RPG épico em três discos.', 1997, 259.90, 2, TRUE, UTC_TIMESTAMP(), 4, 2),                  -- 13
('Crash Bandicoot', 'Plataforma em 3D.', 1996, 109.90, 3, TRUE, UTC_TIMESTAMP(), 4, 2),                            -- 14
('Metal Gear Solid', 'Ação e infiltração.', 1998, 149.90, 2, TRUE, UTC_TIMESTAMP(), 4, 2),                         -- 15
('Super Mario 64', 'Plataforma em 3D no N64.', 1996, 279.90, 2, TRUE, UTC_TIMESTAMP(), 5, 2),                      -- 16
('The Legend of Zelda: Ocarina of Time', 'Aventura 3D.', 1998, 299.90, 1, TRUE, UTC_TIMESTAMP(), 5, 2),            -- 17
('Mario Kart 64', 'Corrida com itens.', 1996, 219.90, 3, TRUE, UTC_TIMESTAMP(), 5, 2),                             -- 18
('Pokémon Red', 'RPG de capturar monstros.', 1996, 199.90, 2, TRUE, UTC_TIMESTAMP(), 6, 2),                        -- 19
('Tetris', 'O puzzle mais famoso.', 1989, 89.90, 6, TRUE, UTC_TIMESTAMP(), 6, 2),                                  -- 20
('Kirby''s Dream Land', 'Plataforma leve para Game Boy.', 1992, 99.90, 1, FALSE, UTC_TIMESTAMP(), 6, 2);          -- 21 (inativo)

-- Associação jogo x gênero (1 Plataforma, 2 Aventura, 3 RPG, 4 Corrida, 5 Luta, 6 Ação, 7 Esporte, 8 Puzzle)
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
(1, 1),
(2, 2), (2, 3),
(3, 1), (3, 6),
(4, 1), (4, 2),
(5, 3), (5, 2),
(6, 5),
(7, 4),
(8, 1), (8, 6),
(9, 6), (9, 5),
(10, 3), (10, 2),
(11, 5),
(12, 4), (12, 7),
(13, 3), (13, 2),
(14, 1), (14, 6),
(15, 6), (15, 2),
(16, 1), (16, 2),
(17, 2), (17, 6),
(18, 4),
(19, 3), (19, 2),
(20, 8),
(21, 1);