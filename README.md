# Munakanplo

Egy nyílt forráskódú munkanaplo vezetését megkönnyítő alkalmazás

<details>
<summary><strong><h2>Telepítése</h2></strong></summary>
<hr>

### Rendszerkövetelmények

- Docker 20+
- Hivatalos Microsoft dotnet docker képek:

  ```bash
  sudo docker pull mcr.microsoft.com/dotnet/aspnet:7.0
  sudo docker pull mcr.microsoft.com/dotnet/sdk:7.0.401
  ```

### Futtatása

1. Projekt klonolása:

```bash
git clone https://github.com/tm01013/Munkanaplo2.git
cd Munkanaplo2
```

2. Docker kép készítése

```bash
sudo docker build -t munkanaplo3 --no-cache .
```

> Ehhez adminisztrátori jogosultságra van szükség

3. Szerver indítása

```bash
sudo docker run --name Munkanaplo -itd -p <port amelyen futtatni akarod>:80 munkanaplo3
```

> Ehhez adminisztrátori jogosultságra van szükség

</details>

## [Használata](/HOWTOUSE.md)

## [Licence](/LICENCE)

© Tatár Márton 2023

Ez a projekt az MIT licence alatt all.

<details>
  
| Lehet                                        | Nem lehet                         | Muszály                                     |
| -------------------------------------------- | --------------------------------- | ------------------------------------------- |
| Kereskedelmi célú felhasználás               | Felelőségre vonni a felesztő(ke)t | A felylesztő(k) Copyright jogát feltüntetni |
| Módosítani                                   |                                   | Tartalmaznia kell az MIT licence-t          |
| Terjeszteni eredeti vagy módosított formában |                                   |                                             |
| Privát használat                             |                                   |                                             |
</details>
