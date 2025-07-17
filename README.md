# 📘 Commandes disponibles — Projet RobotFactory

Ce document récapitule toutes les **commandes utilisateur** disponibles dans l'application console `RobotFactory`, avec des exemples concrets pour faciliter la démonstration.

---

## 🧱 Design Patterns utilisés

### 1. **Factory Method**

> Utilisé pour créer dynamiquement des objets `Factory`, `Robot`, `Piece`, ou `Order` sans exposer leur logique de création dans le `CommandParser`.

✅ Cela permet d’ajouter de nouveaux types d’usines ou de robots facilement.

### 2. **Strategy**

> Implémenté pour gérer les différentes stratégies de validation et génération d'instructions (standard vs modifié).

✅ Par exemple, `InstructionBuilder` peut générer des instructions pour un robot standard ou modifié, sans changer le cœur de la logique de production.

### 3. **Command**

> Chaque commande utilisateur (`PRODUCE`, `VERIFY`, `SEND`, etc.) correspond implicitement à une action encapsulée dans une structure de traitement unique (`CommandParser`).

✅ Ce design facilite l’ajout de nouvelles commandes sans casser le reste du code.

### 4. **Singleton (optionnel)**

> Le `FactoryManager` agit parfois comme un singleton partagé pour gérer toutes les usines.

✅ Cela centralise la gestion multi-usine sans instancier plusieurs fois la même logique.

### 5. **Observer (via logger)**

> Le `StockHistoryLogger` joue un rôle d’observateur passif des actions système (productions, transferts, réceptions, etc.).

✅ Cela permet de tracer toutes les actions sans modifier les classes concernées.

---

## ✅ Commandes de base

### `STOCKS`

> Affiche le stock global des robots et pièces de l'usine par défaut.

```bash
STOCKS
```

### `STOCKS IN UsineX`

> Affiche le stock d'une usine spécifique.

```bash
STOCKS IN Usine1
```

### `RECEIVE ARGS [IN UsineX]`

> Ajoute manuellement des robots ou pièces au stock.

```bash
RECEIVE 2 XM-1, 5 Core_CM1 IN Usine2
```

### `NEEDED_STOCKS ARGS`

> Affiche les pièces nécessaires à la production d'une commande.

```bash
NEEDED_STOCKS 2 RD-1, 1 XM-1
```

### `INSTRUCTIONS ARGS`

> Affiche les étapes de fabrication pour produire les robots spécifiés.

```bash
INSTRUCTIONS 1 XM-1
```

### `VERIFY ARGS`

> Vérifie si la commande est valide et si le stock est suffisant.

```bash
VERIFY 2 RD-1
```

---

## 🛠 Commande de production

### `PRODUCE ARGS [IN UsineX]`

> Fabrique des robots, met à jour le stock, génère les instructions et enregistre dans les logs.

```bash
PRODUCE 1 WI-1 IN Usine1
```

### `PRODUCE ... WITH/WITHOUT/REPLACE ... ; ...`

> Permet de modifier dynamiquement les pièces d’un robot à produire.

```bash
PRODUCE 1 XM-1 REPLACE 1 Arms_AM1, 1 Arms_AD1 WITH 1 Legs_LD1; 2 RD-1 IN Usine2
```

---

## 🧠 Gestion des templates

### `ADD_TEMPLATE TEMPLATE_NAME, Piece1, ..., PieceN`

> Crée un nouveau modèle de robot, validé selon les contraintes de catégorie.

```bash
ADD_TEMPLATE CLEANBOT, Core_CD1, Generator_GD1, Arms_AD1, Legs_LD1, System_SD1
```

---

## 🧾 Gestion des commandes de vente

### `ORDER ARGS`

> Enregistre une commande de robots à livrer.

```bash
ORDER 1 XM-1, 2 RD-1
```

### `SEND ORDERID, ARGS`

> Expédie une partie ou la totalité des robots de la commande.

```bash
SEND ORDER1, 1 XM-1
```

### `LIST_ORDER`

> Affiche la liste des commandes non entièrement livrées.

```bash
LIST_ORDER
```

---

## 🔍 Historique & traçabilité

### `GET_MOVEMENTS`

> Affiche l’historique complet des mouvements de stock.

```bash
GET_MOVEMENTS
```

### `GET_MOVEMENTS ARGS`

> Filtre l’historique sur des éléments précis (robot ou pièce).

```bash
GET_MOVEMENTS XM-1, Core_CM1
```

---

## 🏭 Multi-usines

### `TRANSFER Usine1, Usine2, ARGS`

> Transfère des pièces ou robots d’une usine à une autre.

```bash
TRANSFER Usine1, Usine2, 1 XM-1, 3 Core_CD1
```

> Tous les `PRODUCE`, `RECEIVE`, `STOCKS`, `SAVE`, etc., supportent `IN UsineX`.

---

## 💾 Sauvegarde & chargement

### `SAVE_STOCKS IN UsineX`

> Sauvegarde le stock d’une usine en JSON dans `/exports`.

```bash
SAVE_STOCKS IN Usine1
```

### `SAVE_ALL_STOCKS`

> Sauvegarde tous les stocks de toutes les usines dans `/exports`

```bash
SAVE_ALL_STOCKS
```

### `LOAD_STOCKS IN UsineX FROM path.json`

> Recharge un stock depuis un fichier JSON existant.

```bash
LOAD_STOCKS IN Usine1 FROM exports/stock_usine1.json
```

---

## 📝 Rapport

### `REPORT IN UsineX`

> Affiche un résumé de l’usine : robots, pièces, mouvements récents.

```bash
REPORT IN Usine1
```

---

## 🧪 Test rapide

```bash
RECEIVE 5 Core_CM1, 3 Arms_AM1 IN Usine1
PRODUCE 1 XM-1 IN Usine1
STOCKS IN Usine1
SAVE_STOCKS IN Usine1
REPORT IN Usine1
```

---

🎉 *Fin du README des commandes. Bonnes démonstrations !*
