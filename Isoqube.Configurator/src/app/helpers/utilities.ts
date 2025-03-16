import * as CryptoJS from 'crypto-js';
import { jwtDecode } from "jwt-decode";

export class Utilities {

  static SIGN_TOKEN_KEY: string = 'immigrator';
  static LOCAL_TOKEN_ID: string = 'uas_token';

  static validateEmail(emailString: string): boolean {

    if (/(.+)@(.+){2,}\.(.+){2,}/.test(emailString)) {
      return true;
    }

    return false;
  }

  static signToken(base64Encodedpayload: any, key: string): string {
    var secret = key;
    let token: any = this.encodeToken(base64Encodedpayload);

    var signature: any = CryptoJS.HmacSHA256(token, secret);
    signature = this.base64url(signature);

    var signedToken = token + "." + signature;
    return signedToken;
  }

  static decodeJwt(token: string): any {
    try {
      return atob(jwtDecode(token));
    } catch (Error) {
      return null;
    }
  }

  private static base64url(source: any): string {

    let encodedSource = CryptoJS.enc.Base64.stringify(source);

    encodedSource = encodedSource.replace(/=+$/, '');

    encodedSource = encodedSource.replace(/\+/g, '-');
    encodedSource = encodedSource.replace(/\//g, '_');

    return encodedSource;
  }

  private static encodeToken(payload: any): string {
    var header = {
      "alg": "HS256",
      "typ": "JWT"
    };

    var stringifiedHeader = CryptoJS.enc.Utf8.parse(JSON.stringify(header));
    var encodedHeader = this.base64url(stringifiedHeader);

    var stringifiedData = CryptoJS.enc.Utf8.parse(JSON.stringify(payload));
    var encodedData = this.base64url(stringifiedData);

    return encodedHeader + "." + encodedData;
  }
}
