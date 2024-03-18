# Galapagos Book API

株式会社ガラパゴスの採用テストで提出するリポジトリです。

## プロジェクト構成

- [src](/src)
  - [BookApi.Domain](/src/BookApi.Domain/): ビジネスロジックを担当するドメイン層
  - [BookApi.Infrastructure](/src/BookApi.Infrastructure/): DBとやり取りするインフラ層
  - [BookApi.UseCase](/src/BookApi.UseCase/): コントローラから呼び出すユースケース層
  - [BookApi.Presentation](/src/BookApi.Presentation/): APIのプレゼンテーション層
  - [BookApi.Test](/src/BookApi.Test/): ユースケース単位のユニットテスト

- [Diagrams](/Diagrams/): 設計書 (ドメインモデル図及びER図)

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

## 動作手順

### コンテナの起動

リポジトリのルートディレクトリでDockerコンテナを起動します。

```bash
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

APIの仕様ドキュメントは [/openapi.json](/openapi.json) にあります。

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

詳しい仕様に関しては、[Swagger UI](http://localhost:5000/swagger) もしくは [付属のOpenAPIドキュメント](./openapi.json) を参照してください。
