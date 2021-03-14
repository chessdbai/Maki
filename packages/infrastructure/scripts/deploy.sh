#!/bin/bash

set -euxo pipefail

PROFILE_NAME=chessdb-staging
ACCOUNT_ID=407299974961
REGION=us-east-2

npm run build
cdk bootstrap aws://$ACCOUNT_ID/$REGION --profile $PROFILE_NAME
cdk deploy --profile $PROFILE_NAME --require-approval never AmiBuilderStack