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

カテゴリ         |言語・フレームワーク|バージョン|
-----------------|--------------------|----------|
プラットフォーム |.NET                |8.0.0     |
言語             |C#                  |12.0      |
Webフレームワーク|ASP.NET Core        |8.0.0     |
O/Rマッパー      |Entity Framework Core|8.0.3    |
DB               |SQLite              |3.43.2    |
コンテナ         |Docker Compose      |2.23.3    |

<!-- DBはSQLiteじゃなくてポスグレにするかも -->

## 動作手順

ここに動作手順を記入

## 入出力の仕様

ここにAPIの入出力の仕様を記入  

<!-- JSONを列挙していくので、それなりの量になりそう。別ファイルに分けるべきかも？ -->
