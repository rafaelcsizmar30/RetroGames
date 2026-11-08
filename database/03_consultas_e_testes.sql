USE retrogames;

-- 1. Catálogo: jogos ativos com plataforma e condição, ordenados por nome
SELECT j.co_seq_jogo, j.no_jogo, p.no_plataforma, c.ds_condicao_jogo, j.vl_preco
FROM tb_jogo j
INNER JOIN tb_plataforma p ON p.co_seq_plataforma = j.co_plataforma
INNER JOIN tb_condicao_jogo c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2. Gêneros de um jogo informado (exemplo: jogo 2)
SELECT g.no_genero
FROM tb_jogo_genero jg
INNER JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = 2;

-- 3. Jogos de um gênero informado (exemplo: gênero 3, RPG)
SELECT j.no_jogo
FROM tb_jogo_genero jg
INNER JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = 3
ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma, incluindo plataformas sem jogos
SELECT p.no_plataforma, COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, 5 itens por página (página 1; página 2 = OFFSET 5)
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo LIKE '%Mario%'
ORDER BY no_jogo
LIMIT 5 OFFSET 0;



-- T1. Jogo com plataforma inexistente. Esperado: erro 1452 (chave estrangeira)
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste FK', 2000, 10.00, 1, UTC_TIMESTAMP(), 999, 1);
-- Resultado: Erro - 1452


-- T2. Plataforma com nome já cadastrado. Esperado: erro 1062 (UNIQUE)
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Super Nintendo', 'Nintendo', 1990);
-- Resultado: Erro - 1062

-- T3. Excluir plataforma que possui jogos. Esperado: erro 1451 (chave estrangeira)
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 1;
-- Resultado: Erro - 1451

-- T4. Jogo com no_jogo nulo. Esperado: erro 1048 (NOT NULL)
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES (NULL, 2000, 10.00, 1, UTC_TIMESTAMP(), 1, 1);
-- Resultado: Erro - 1048

-- T5. Jogo com vl_preco = 0. Esperado: ACEITO (a regra fica no Service, RN06)
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste preco zero', 2000, 0, 1, UTC_TIMESTAMP(), 1, 1);
-- Resultado: Deu certo