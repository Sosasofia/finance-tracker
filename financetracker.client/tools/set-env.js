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
  apiUrl: '${process.env.NG_APP_API_URL}'
};
`;
console.log("🔧 NG_APP_API_URL =", process.env.NG_APP_API_URL);

fs.writeFileSync(targetPath, envConfigFile, { encoding: "utf8" });
console.log(`✅ Environment file generated at ${targetPath}`);
