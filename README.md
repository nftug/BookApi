# Galapagos Book API

株式会社ガラパゴスの採用テストで提出するリポジトリです。

## プロジェクト構成

- [src](/src)
  - [BookApi.Domain](/src/BookApi.Domain/): ビジネスロジックを担当するドメイン層
  - [BookApi.Infrastructure](/src/BookApi.Infrastructure/): DBとやり取りするインフラ層
  - [BookApi.UseCase](/src/BookApi.UseCase/): コントローラから呼び出すユースケース層
  - [BookApi.Presentation](/src/BookApi.Presentation/): APIのプレゼンテーション層
  - [BookApi.Test](/src/BookApi.Test/): ユースケース単位のユニットテスト

- [Docker](/Docker/): Docker Composeの設定ファイル

- [Diagrams](/Diagrams/): 設計書 (ドメインモデル図及びER図)

## ドメインモデル図

![ドメインモデル図](/Diagrams/BookApi_DomainModel.svg)

## 動作環境

下記の環境で動作確認済みです。

カテゴリ         |言語・フレームワーク |バージョン|
--------------------|---------------------|----------|
プラットフォーム    |.NET                 |8.0.0     |
言語                |C#                   |12.0      |
Webフレームワーク   |ASP.NET Core         |8.0.0     |
O/Rマッパー         |Entity Framework Core|8.0.3     |
DB                  |PostgreSQL           |16.2      |
テストフレームワーク|xUnit                |2.4.2     |
コンテナ            |Docker               |24.0.4    |

## 開発環境への入り方

### 開発用コンテナの起動

開発環境のDockerコンテナを起動します。

```bash
cd ./Docker/bookapi-development
docker-compose up -d
```

MacOSでDocker Desktopの代替にLimaを使用している場合、事前にマウントの書き込み許可を設定する必要があります。

`~/.lima/docker/lima.yaml` を開き、`mounts` の項目を下記の例に倣って編集してください。

```yaml
mounts:
  - location: '~'
  - location: '~/Projects/GalapagosBookApi/src'  # このディレクトリ以下への書き込みを許可する
    writable: true
  - location: '/tmp/lima'
    writable: true
```

### VSCodeでアタッチ

VSCodeで `bookapi-dev` コンテナにアタッチし、 `/src/` ディレクトリを開きます。

画面右端にC# Dev Kitの拡張機能のインストールを促すポップアップが表示されます。指示に従ってインストールしてください。

### デバッグ実行

コンテナ内で下記のコマンドを実行すると、APIをデバッグ実行できます。

```bash
dotnet watch run --project /src/BookApi.Presentation/
```

### テストの実行

ユースケースの自動テストはVSCodeのテストエクスプローラから実行できます。

## 動作手順

### 実行用コンテナの起動

実行用のDockerコンテナを起動します。

```bash
cd ./Docker/bookapi-production
docker-compose up -d
```

### ブラウザで動作確認

ブラウザで <http://localhost:5000/swagger> にアクセスすると、Swagger UIでAPIの操作ができます。

### 直接操作して動作確認

下記のエンドポイントにアクセスしてAPIを直接操作することもできます。

- <http://localhost:5000/api/books/{ISBNCode}>: 書籍のデータを操作
- <http://localhost:5000/api/authors/{Id}>: 著者のデータを操作
- <http://localhost:5000/api/publishers/{Id}>: 出版社のデータを操作

## APIドキュメント

APIの仕様ドキュメントはソースディレクトリ内の [openapi.json](/src/openapi.json) にあります。

ドキュメントはOpenAPI Specification対応のJSON形式です。Postmanなどの対応するアプリケーションでインポートしてお使いください。

## 仕様について

### ISBNコードのフォーマット

書籍リソースはISBNコードで指定します。許容するフォーマットは下記の通りです。

- 13桁、ハイフン付き: `000-0-00-000000-0`
- 10桁、ハイフン付き: `0-00-000000-0`
- 13桁、ハイフン無し: `0000000000000`
- 10桁、ハイフン無し: `0000000000`

入力されたISBNコードのハイフンはすべて無視され、再フォーマットされて処理されます。

- 例: `000--0-0-00000-000`を入力 → `000-0-00-000000-0` として処理

### その他の仕様について

詳しい仕様に関しては、[Swagger UI](http://localhost:5000/swagger) もしくは [付属のOpenAPIドキュメント](/src/openapi.json) を参照してください。
