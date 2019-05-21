export interface ICaseEditor {
  title: string;
  keywords: string[];
  summary: string;
  jurisdiction: INomenclature;
  court: string;
  courtEng: string;
  dateOfDocument: Date;
  language: INomenclature;
  decisionType: INomenclature;
  nationalIdentifier: string;
  ecli: string;
  source: string;
  sourceUrl: string;
  interlexOntology: string;
  eurovoc: INomenclature[];
  euProvisions: IEulawLink[];
  euCaselaw: IEucaseLink[];
  internationalLaw: string;
  internationalCaseLaw: string;
  nationalLawRecords: NationalLawRecord[];
  nationalCaseLawRecords: NationalCaseLawRecord[];
  text: string;
  nationalities: string[];
  domiciles: string[];
  residenceMsForum: INomenclature;
  choiceCourt: INomenclature;
  choiceLaw: INomenclature;
}

export interface ICaseMetaContentResponse {
  content: string;
  editable: boolean;
}

export interface IMetaEditor {
  title?: string;
  titleEn?: string;
  shortTitle?: string;
  abbreviation?: string;
  jurisdiction?: INomenclature;
  language?: INomenclature;
  actType?: INomenclature;
  eli?: string;
  publReference?: string;
  dateOfDocument?: Date;
  dateOfPublication?: Date;
  dateOfEffect?: Date;
  dateEndValidity?: Date;
  sourceName?: string;
  actUrl?: string;
  translatedActUrl?: string;
  actFile?: string; // this needs thinking over
  translatedActFile?: string;
  version?: INomenclature;
  versionDateOfEffect?: Date;
  subsequentAmmendments?: INomenclature;
  infoConsolidatedVersions?: string[];
}

export interface IFile {
  base64Content: string;
  filename: string;
  mimeType: string;
}

export interface NationalLawRecord {
  title: string;
  link: string;
}

export interface NationalCaseLawRecord extends NationalLawRecord {
  identifier: string;
}

export abstract class IEubaseLink {
  celex = '';
  name = '';
  importance: INomenclature = {label: 'Low', code: '0'};
  docNumber: number = null;
  year: number = null;
  // abstract getPartStr(): string;
}

export class IEulawLink extends IEubaseLink {
  docType = 0;
  itemsBase: IEuLinkItem[] = JSON.parse(JSON.stringify(emptyBaseEuParts));
  itemsOther: IEuLinkItem[] = JSON.parse(JSON.stringify(emptyOtherEuParts));
}

export class IEucaseLink extends IEubaseLink {
  part: '';
  preliminaryRuling: INomenclature = {label: 'No', code: '0'};
  court: INomenclature = {label: 'Court of Justice', code: 'C'};
  docType: INomenclature = {label: 'Judgment', code: 'J'};
}


export class IEuLinkItem {
  label: string;
  value: string;
  hrefPart: string;
  placeholder: string;
}

export const emptyBaseEuParts: IEuLinkItem[] = [
  {label: 'Article', value: '', hrefPart: 'Art', placeholder: '12'},
  {label: 'Paragraph', value: '', hrefPart: 'Par', placeholder: '3'},
  {label: 'Alinea', value: '', hrefPart: 'Al', placeholder: '1'},
  {label: 'Point', value: '', hrefPart: 'Pt', placeholder: '6'},
  {label: 'Letter', value: '', hrefPart: 'Let', placeholder: '5'},
  {label: 'Indent', value: '', hrefPart: 'Ind', placeholder: 'third'},
  {label: 'Sentence', value: '', hrefPart: 'Sent', placeholder: 'second'},
  {label: 'Annex', value: '', hrefPart: 'Ann', placeholder: 'Annex II, Article 1(2)(b)'},
  {label: 'Recital', value: '', hrefPart: 'Rec', placeholder: '45'},
];
export const emptyOtherEuParts: IEuLinkItem[] = [
  {label: 'Part', value: '', hrefPart: 'Part', placeholder: 'IV'},
  {label: 'Title', value: '', hrefPart: 'Tit', placeholder: 'III'},
  // { label: 'Subparagraph', value: '', hrefPart: 'Subpar' },
  {label: 'Chapter', value: '', hrefPart: 'Chap', placeholder: '6'},
  // { label: 'Proposal', value: '', hrefPart: 'Prop' },
  {label: 'Section', value: '', hrefPart: 'Sec', placeholder: '4'},
  // { label: 'Reference', value: '', hrefPart: 'Ref' },
];

export interface INomenclature {
  label: string;
  code: string;
}

export const countryList: INomenclature[] = [
  {label: 'European union', code: 'EU'},
  {label: 'Austria', code: 'AT'},
  {label: 'Bulgaria', code: 'BG'},
  {label: 'Czech Republic', code: 'CS'},
  {label: 'France', code: 'FR'},
  {label: 'Germany', code: 'DE'},
  {label: 'Ireland', code: 'EN'},
  {label: 'Italy', code: 'IT'},
  {label: 'Poland', code: 'PL'},
  {label: 'Romania', code: 'RO'},
  {label: 'Spain', code: 'ES'},
  {label: 'Slovakia', code: 'SK'},
  {label: 'Sweden', code: 'SV'},
];

export const languageList: INomenclature[] = [
  {label: 'Bulgarian', code: 'BG'},
  {label: 'Czech', code: 'CS'},
  {label: 'English', code: 'EN'},
  {label: 'German', code: 'DE'},
  {label: 'French', code: 'FR'},
  {label: 'Italian', code: 'IT'},
  {label: 'Polish', code: 'PL'},
  {label: 'Romanian', code: 'RO'},
  {label: 'Spanish', code: 'ES'},
  {label: 'Slovak', code: 'SK'},
  {label: 'Swedish', code: 'SV'},
];

export const nationalities = ['Afghan', 'Albanian', 'Algerian', 'American', 'Argentinian', 'Australian', 'Austrian', 'Bangladeshi', 'Batswana',
  'Belgian', 'Bolivian', 'Brazilian', 'British', 'Bulgarian', 'Cambodian', 'Cameroonian', 'Canadian', 'Chilean', 'Chinese', 'Colombian',
  'Costa Rican', 'Croatian', 'Cuban', 'Czech', 'Danish', 'Dominican', 'Dutch', 'Ecuadorian', 'Egyptian', 'Emirati', 'English',
  'Estonian', 'Ethiopian', 'Fijian', 'Finnish', 'French', 'German', 'Ghanaian', 'Greek', 'Guatemalan', 'Haitian', 'Honduran',
  'Hungarian', 'Icelandic', 'Indian', 'Indonesian', 'Iranian', 'Iraqi', 'Irish', 'Israeli', 'Italian', 'Jamaican', 'Japanese',
  'Jordanian', 'Kenyan', 'Korean', 'Kuwaiti', 'Lao', 'Latvian', 'Lebanese', 'Libyan', 'Lithuanian', 'Luxembourgian', 'Malaysian', 'Malian', 'Maltese',
  'Mexican', 'Mongolian', 'Moroccan', 'Mozambican', 'Namibian', 'Nepalese', 'New', 'Zealand', 'Nicaraguan', 'Nigerian', 'Norwegian',
  'Pakistani', 'Panamanian', 'Paraguayan', 'Peruvian', 'Philippine', 'Polish', 'Portuguese', 'Romanian', 'Russian', 'Salvadorian',
  'Saudi', 'Scottish', 'Senegalese', 'Serbian', 'Singaporean', 'Slovak', 'South', 'African', 'Spanish', 'Sri', 'Lankan', 'Sudanese',
  'Swedish', 'Swiss', 'Syrian', 'Taiwanese', 'Tajikistani', 'Thai', 'Tongan', 'Tunisian', 'Turkish', 'Ukrainian', 'Uruguayan',
  'Venezuelan', 'Vietnamese', 'Welsh', 'Zambian', 'Zimbabwean'];

export const countries = ["Afghanistan", "Albania", "Algeria", "American Samoa", "Andorra", "Angola", "Anguilla",
  "Antarctica", "Antigua and Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas",
  "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia",
  "Bosnia and Herzegowina", "Botswana", "Bouvet Island", "Brazil", "British Indian Ocean Territory", "Brunei Darussalam",
  "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Canada", "Cape Verde", "Cayman Islands",
  "Central African Republic", "Chad", "Chile", "China", "Christmas Island", "Cocos (Keeling) Islands", "Colombia",
  "Comoros", "Congo", "Congo, the Democratic Republic of the", "Cook Islands", "Costa Rica", "Cote d'Ivoire",
  "Croatia (Hrvatska)", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic",
  "East Timor", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia",
  "Falkland Islands (Malvinas)", "Faroe Islands", "Fiji", "Finland", "France", "France Metropolitan", "French Guiana",
  "French Polynesia", "French Southern Territories", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar",
  "Greece", "Greenland", "Grenada", "Guadeloupe", "Guam", "Guatemala", "Guinea", "Guinea-Bissau", "Guyana", "Haiti",
  "Heard and Mc Donald Islands", "Holy See (Vatican City State)", "Honduras", "Hong Kong", "Hungary", "Iceland", "India",
  "Indonesia", "Iran (Islamic Republic of)", "Iraq", "Ireland", "Israel", "Italy", "Jamaica", "Japan", "Jordan",
  "Kazakhstan", "Kenya", "Kiribati", "Korea, Democratic People's Republic of", "Korea, Republic of", "Kuwait",
  "Kyrgyzstan", "Lao, People's Democratic Republic", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libyan Arab Jamahiriya",
  "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia, The Former Yugoslav Republic of", "Madagascar",
  "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Martinique", "Mauritania", "Mauritius",
  "Mayotte", "Mexico", "Micronesia, Federated States of", "Moldova, Republic of", "Monaco", "Mongolia", "Montserrat",
  "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands", "Netherlands Antilles",
  "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Niue", "Norfolk Island", "Northern Mariana Islands",
  "Norway", "Oman", "Pakistan", "Palau", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Pitcairn",
  "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russian Federation", "Rwanda",
  "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent and the Grenadines", "Samoa", "San Marino",
  "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Seychelles", "Sierra Leone", "Singapore",
  "Slovakia (Slovak Republic)", "Slovenia", "Solomon Islands", "Somalia", "South Africa",
  "South Georgia and the South Sandwich Islands", "Spain", "Sri Lanka", "St. Helena", "St. Pierre and Miquelon",
  "Sudan", "Suriname", "Svalbard and Jan Mayen Islands", "Swaziland", "Sweden", "Switzerland", "Syrian Arab Republic",
  "Taiwan, Province of China", "Tajikistan", "Tanzania, United Republic of", "Thailand", "Togo", "Tokelau", "Tonga",
  "Trinidad and Tobago", "Tunisia", "Türkiye", "Turkmenistan", "Turks and Caicos Islands", "Tuvalu", "Uganda", "Ukraine",
  "United Arab Emirates", "United Kingdom", "United States", "United States Minor Outlying Islands", "Uruguay",
  "Uzbekistan", "Vanuatu", "Venezuela", "Vietnam", "Virgin Islands (British)", "Virgin Islands (U.S.)",
  "Wallis and Futuna Islands", "Western Sahara", "Yemen", "Yugoslavia", "Zambia", "Zimbabwe"];

export const decisionTypes: INomenclature[] = [
  {label: 'Judgment', code: 'J'},
  {label: 'Order', code: 'O'},
  {label: 'Opinion of the Advocate-General', code: 'C'},
  {label: 'Other', code: 'other'}
];

export interface ICaseListResponseModel {
  caseId: number;
  userName: string;
  title: string;
  lastChange: Date;
  organization: string;
  docDate: Date;
  editable: boolean;
}

export interface ICaseListRequestModel {
  userName: string;
  pageNumber: number;
  count: number;
  organization: string;
  jurisdictionCode: string;
}

export const importance: INomenclature[] = [
  {label: 'High', code: '1'},
  {label: 'Low', code: '0'},
];

export const yesNoRule: INomenclature[] = [
  {label: 'Yes', code: '1'},
  {label: 'No', code: '0'},
];

// export function padStart(txt: string, targetLength: number, padString: string) {
//     padString = String((typeof padString !== 'undefined' ? padString : ' '));
//     if (txt.length > targetLength) {
//         return txt;
//     } else {
//         targetLength = targetLength - txt.length;
//         if (targetLength > padString.length) {
//             padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
//         }
//         return padString.slice(0, targetLength) + txt;
//     }
// }
