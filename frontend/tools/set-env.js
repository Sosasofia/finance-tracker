const fs = require("fs");
const path = require("path");
const dotenv = require("dotenv");
const dotenvExpand = require("dotenv-expand");

const env = dotenv.config();
dotenvExpand.expand(env);

const targetPath = "./src/environments/environment.prod.ts";
const envConfigFile = `
export const environment = {
  production: true,
  apiUrl: '${process.env.NG_APP_API_URL}',
  googleClientId: '${process.env.NG_APP_GOOGLE_CLIENT_ID}',
};
`;

fs.writeFileSync(targetPath, envConfigFile, { encoding: "utf8" });
console.log(`Environment file generated at ${targetPath}`);
