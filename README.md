# Munakanplo

Egy nyílt forráskódú munkanaplo vezetését megkönnyítő alkalmazás

<details>
<summary><strong><h2>Telepítése</h2></strong></summary>
<hr>

### Rendszerkövetelmények

- Docker 20+

### Futtatása

> Ehhez adminisztrátori jogosultságra van szükség!

1. Docker kép letöltése:
    ```bash
    sudo docker pull tm01013/munkanaplo3
    ```

2. Szerver indítása:
    ```bash
    sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo3
    ```

<details>
<summary><h3>Frissítés korábbi verzióról</h3></summary>

> Ehhez adminisztrátori jogosultságra van szükség!

1. Adatbázis kimásolása a régi konténerből
    ```bash
    sudo docker cp <régi konténer neve>:/app.db ~/app.db
    ```
2. Régi konténer törlése
    ```bash
    sudo docker remove <régi konténer neve>
    ```
3. Új konténer telepítése
    ```bash
    sudo docker pull tm01013/munkanaplo3
    sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo3
    ```
4. Adatbázis bemásolása az új konténerbe
    ```bash
    sudo docker cp ~/app.db Munkanaplo:/app.db
    ```

</details>

<details>
<summary><h4>Telepítés forráskódból</h4></summary>

> Ehhez adminisztrátori jogosultságra van szükség!

1. Projekt klonolása:
    ```bash
    git clone https://github.com/tm01013/Munkanaplo2.git
    cd Munkanaplo2
    ```

2. Microsoft aspnet és dotnet sdk letöltése:
    ```bash
    sudo docker pull mcr.microsoft.com/dotnet/aspnet:7.0
    sudo docker pull mcr.microsoft.com/dotnet/sdk:7.0.401
    ```

3. Docker kép készítése
    ```bash
    sudo docker build -t munkanaplo3 --no-cache .
    ```

4. Szerver indítása
    ```bash
    sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo3
    ```

</details>

</details>

## [Használata](/HOWTOUSE.md)

## [Licence](/LICENCE)

© Tatár Márton 2023

Ez a projekt az MIT licence alatt all.

| Lehet                                        | Nem lehet                         | Muszály                                     |
| -------------------------------------------- | --------------------------------- | ------------------------------------------- |
| Kereskedelmi célú felhasználás               | Felelőségre vonni a felesztő(ke)t | A felylesztő(k) Copyright jogát feltüntetni |
| Módosítani                                   |                                   | Tartalmaznia kell az MIT licence-t          |
| Terjeszteni eredeti vagy módosított formában |                                   |                                             |
| Privát használat                             |                                   |                                             |
