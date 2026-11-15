CREATE TABLE clientes (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome varchar(50) NOT NULL,
    cpf char(11) NOT NULL UNIQUE,
    email varchar(50),
    data_nascimento date NOT NULL,

    CONSTRAINT ck_clientes_cpf_digitos
        CHECK (cpf ~ '^[0-9]{11}$')
);

CREATE TABLE dividas (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    cliente_id integer NOT NULL,
    valor numeric(10, 2) NOT NULL,
    paga boolean NOT NULL DEFAULT false,
    data_criacao timestamp without time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_pagamento timestamp without time zone,

    FOREIGN KEY (cliente_id) REFERENCES clientes(id),
    CHECK (valor > 0)
);