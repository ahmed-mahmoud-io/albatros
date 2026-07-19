/* ALBATROS — shared i18n engine (EN / AR) */
const translations = {
  en: {
    nav_features:"Features", nav_properties:"Properties", nav_contact:"Contact",
    nav_search:"Search now", nav_login:"Login", nav_signup:"Sign up",

    hero_eyebrow:"Riyadh · Jeddah · Dammam",
    hero_title:"Find a home that <em>understands</em> your ambition.",
    hero_lead:"ALBATROS curates the Kingdom's finest residences and investment properties — verified listings, transparent pricing, and a team that closes deals with precision.",
    hero_cta1:"Browse Properties", hero_cta2:"Book a Consultation",
    stat1:"Properties Available", stat2:"Happy Customers", stat3:"Countries in Region", stat4:"Customer Satisfaction",

    tag_sale:"For Sale", tag_rent:"For Rent", tag_new:"New",
    sar:"SAR", sar_yr:"SAR / yr", view_details:"View Details",
    beds2:"2 Beds", beds3:"3 Beds", beds4:"4 Beds", beds5:"5 Beds", beds6:"6 Beds",
    baths2:"2 Baths", baths3:"3 Baths", baths4:"4 Baths", baths5:"5 Baths",

    prop1_title:"Al Narjis Villa Residence", prop1_loc:"Al Narjis, Riyadh",
    prop2_title:"Olaya Sky Apartment", prop2_loc:"Olaya District, Riyadh",
    prop3_title:"Al Yasmin Signature Estate", prop3_loc:"Al Yasmin, Riyadh",
    prop4_title:"Al Malqa Garden Villa", prop4_loc:"Al Malqa, Riyadh",
    prop5_title:"King Abdullah Financial Loft", prop5_loc:"KAFD, Riyadh",
    prop6_title:"Diplomatic Quarter Mansion", prop6_loc:"Diplomatic Quarter, Riyadh",
    prop7_title:"Al Rabwah Family Home", prop7_loc:"Al Rabwah, Riyadh",
    prop8_title:"Hittin Modern Townhouse", prop8_loc:"Hittin, Riyadh",
    prop9_title:"Al Sahafa Corner Villa", prop9_loc:"Al Sahafa, Riyadh",

    why_eyebrow:"Why Albatros", why_title:"Built for buyers, sellers and investors who move fast.",
    feat1_h:"Verified Listings", feat1_p:"Every property is inspected and legally verified before it appears on ALBATROS.",
    feat2_h:"Transparent Pricing", feat2_p:"No hidden fees. See market comparisons before you make an offer.",
    feat3_h:"Smart Search & Filters", feat3_p:"Narrow thousands of listings by district, budget, size and yield in seconds.",
    feat4_h:"Dedicated Advisors", feat4_p:"A personal consultant guides you from first viewing to closing.",
    cta_h:"Ready to find your next property?", cta_p:"Book a free consultation with an ALBATROS advisor today.",

    foot_desc:"A premium real estate platform connecting buyers, sellers and investors across Saudi Arabia with verified, high-value properties.",
    foot_links:"Quick Links", foot_home:"Home", foot_support:"Support",
    foot_help:"Help Center", foot_terms:"Terms of Service", foot_privacy:"Privacy Policy", foot_faq:"FAQs",
    foot_contact:"Contact", foot_address:"King Fahd Road, Riyadh, Saudi Arabia",
    foot_rights:"© 2026 ALBATROS. All rights reserved.", foot_tagline:"Designed for the Saudi real estate market",

    /* properties.html */
    bc_home:"Home", bc_properties:"Properties",
    prop_page_title:"Explore Our Properties",
    prop_page_lead:"Browse verified listings across Saudi Arabia's most sought-after districts.",
    f_title:"Filter Properties", f_location:"Location", f_all_locations:"All Locations",
    f_type:"Property Type", f_all_types:"All Types", f_villa:"Villa", f_apartment:"Apartment", f_townhouse:"Townhouse",
    f_status:"Status", f_all_status:"All", f_forsale:"For Sale", f_forrent:"For Rent",
    f_price:"Price Range (SAR)", f_min:"Min", f_max:"Max",
    f_beds:"Bedrooms", f_any:"Any",
    f_apply:"Apply Filters", f_reset:"Reset",
    results_showing:"Showing", results_of:"of", results_properties:"properties",
    sort_label:"Sort by", sort_newest:"Newest", sort_low:"Price: Low to High", sort_high:"Price: High to Low",
    pg_prev:"Previous", pg_next:"Next",

    /* property-details.html */
    d_back:"Back to Properties",
    d_beds:"Bedrooms", d_baths:"Bathrooms", d_area:"Area", d_year:"Year Built",
    d_desc_h:"Description",
    d_desc_p:"This exceptional residence blends contemporary architecture with timeless comfort. Featuring expansive living spaces, premium finishes and a private landscaped garden, it offers an elevated lifestyle in one of Riyadh's most desirable neighborhoods — minutes from schools, retail and business districts.",
    d_amenities_h:"Amenities",
    am1:"Private Garden", am2:"Covered Parking (2)", am3:"Smart Home System", am4:"Central A/C",
    am5:"Maid's Room", am6:"Security & CCTV", am7:"Swimming Pool", am8:"Elevator Access",
    d_agent_h:"Listing Agent", d_agent_name:"Faisal Al-Otaibi", d_agent_role:"Senior Property Advisor",
    d_request_viewing:"Request a Viewing", d_call:"Call Agent",
    d_related_h:"Similar Properties",

    /* contact.html */
    c_eyebrow:"Get in Touch", c_title:"Let's find your next property together.",
    c_lead:"Whether you're buying, selling or investing — our advisors respond within one business day.",
    c_form_h:"Send us a message",
    c_name:"Full Name", c_email:"Email Address", c_phone:"Phone Number", c_subject:"I'm interested in",
    c_subject_buy:"Buying", c_subject_sell:"Selling", c_subject_rent:"Renting", c_subject_other:"Other",
    c_message:"Message", c_message_ph:"Tell us what you're looking for...",
    c_send:"Send Message",
    c_info_h:"Office Information",
    c_office:"Riyadh Head Office", c_hours_h:"Working Hours", c_hours:"Sunday – Thursday, 9:00 AM – 6:00 PM",
    c_map_note:"Interactive map coming soon",

    /* login.html */
    auth_visual_h:"Your next property is one login away.",
    auth_visual_p:"Access saved searches, book viewings, and message advisors directly from your ALBATROS account.",
    login_h:"Welcome back", login_sub:"Log in to manage your properties and viewings.",
    login_pass:"Password", login_remember:"Remember me", login_forgot:"Forgot password?",
    login_btn:"Log In", login_or:"or continue with",
    login_noaccount:"Don't have an account?", login_signup_link:"Sign up",

    /* register.html */
    reg_visual_h:"Join thousands finding their perfect property.",
    reg_visual_p:"Create a free account to save favorites, track price changes, and get matched with new listings first.",
    reg_h:"Create your account", reg_sub:"Join ALBATROS free and start exploring properties.",
    reg_confirm:"Confirm Password", reg_terms:"I agree to the Terms & Privacy Policy",
    reg_btn:"Create Account", reg_hasaccount:"Already have an account?", reg_login_link:"Log in",
    reg_iam:"I am a", reg_buyer:"Buyer", reg_buyer_sub:"Looking to buy or rent",
    reg_seller:"Seller", reg_seller_sub:"Looking to list a property",
    splash_skip:"Click to skip",

    /* admin.html */
    adm_overview:"Overview", adm_properties:"Properties", adm_leads:"Leads", adm_users:"Users",
    adm_backsite:"Back to website", adm_role:"Administrator",
    adm_stat_props:"Total Properties", adm_stat_active:"Active Listings",
    adm_stat_leads:"New Leads (30d)", adm_stat_users:"Registered Users",
    adm_chart_title:"Properties Listed Per Month", adm_activity_title:"Recent Activity",
    adm_search_props:"Search properties...", adm_add_property:"Add Property",
    adm_th_property:"Property", adm_th_type:"Type", adm_th_price:"Price", adm_th_status:"Status", adm_th_actions:"Actions",
    adm_search_leads:"Search leads...", adm_all:"All", adm_new:"New", adm_contacted:"Contacted", adm_closed:"Closed",
    adm_th_name:"Name", adm_th_contact:"Contact", adm_th_interest:"Interested In", adm_th_date:"Date",
    adm_search_users:"Search users...", adm_th_user:"User", adm_th_role:"Role", adm_th_joined:"Joined",
    adm_f_title:"Title", adm_f_price:"Price (SAR)", adm_f_listing_status:"Listing Status",
    adm_active:"Active", adm_pending:"Pending", adm_f_image:"Image URL", adm_save:"Save Property",

    /* favorites.html */
    fav_title:"My Favorites", fav_lead:"Properties you've saved for later.",
    fav_empty_h:"No favorites yet", fav_empty_p:"Tap the heart icon on any property to save it here.",
    fav_browse:"Browse Properties", nav_favorites:"Favorites"
  },

  ar: {
    nav_features:"المميزات", nav_properties:"العقارات", nav_contact:"تواصل معنا",
    nav_search:"ابحث الآن", nav_login:"تسجيل الدخول", nav_signup:"إنشاء حساب",

    hero_eyebrow:"الرياض · جدة · الدمام",
    hero_title:"اعثر على منزل <em>يفهم</em> طموحك.",
    hero_lead:"ألباتروس يقدّم أرقى العقارات السكنية والاستثمارية في المملكة — قوائم موثقة، أسعار شفافة، وفريق ينجز الصفقات بدقة واحترافية.",
    hero_cta1:"تصفح العقارات", hero_cta2:"احجز استشارة",
    stat1:"عقار متاح", stat2:"عميل سعيد", stat3:"دول في المنطقة", stat4:"نسبة رضا العملاء",

    tag_sale:"للبيع", tag_rent:"للإيجار", tag_new:"جديد",
    sar:"ريال", sar_yr:"ريال / سنوياً", view_details:"عرض التفاصيل",
    beds2:"غرفتا نوم", beds3:"3 غرف نوم", beds4:"4 غرف نوم", beds5:"5 غرف نوم", beds6:"6 غرف نوم",
    baths2:"حمامان", baths3:"3 حمامات", baths4:"4 حمامات", baths5:"5 حمامات",

    prop1_title:"فيلا النرجس السكنية", prop1_loc:"حي النرجس، الرياض",
    prop2_title:"شقة العليا العصرية", prop2_loc:"حي العليا، الرياض",
    prop3_title:"عقار الياسمين المميز", prop3_loc:"حي الياسمين، الرياض",
    prop4_title:"فيلا الملقا مع حديقة", prop4_loc:"حي الملقا، الرياض",
    prop5_title:"لوفت المركز المالي", prop5_loc:"المركز المالي، الرياض",
    prop6_title:"قصر الحي الدبلوماسي", prop6_loc:"الحي الدبلوماسي، الرياض",
    prop7_title:"منزل الروضة العائلي", prop7_loc:"حي الروضة، الرياض",
    prop8_title:"تاون هاوس حطين العصري", prop8_loc:"حي حطين، الرياض",
    prop9_title:"فيلا الصحافة الركنية", prop9_loc:"حي الصحافة، الرياض",

    why_eyebrow:"لماذا ألباتروس", why_title:"مصمم للمشترين والبائعين والمستثمرين الذين يتحركون بسرعة.",
    feat1_h:"قوائم موثقة", feat1_p:"يتم فحص كل عقار والتحقق منه قانونياً قبل عرضه على ألباتروس.",
    feat2_h:"أسعار شفافة", feat2_p:"بدون رسوم خفية. قارن الأسعار في السوق قبل تقديم عرضك.",
    feat3_h:"بحث وفلاتر ذكية", feat3_p:"صفّي آلاف العقارات حسب الحي والميزانية والمساحة والعائد خلال ثوانٍ.",
    feat4_h:"مستشارون مخصصون", feat4_p:"مستشار شخصي يرافقك من أول معاينة وحتى إغلاق الصفقة.",
    cta_h:"جاهز تلاقي عقارك القادم؟", cta_p:"احجز استشارة مجانية مع أحد مستشاري ألباتروس اليوم.",

    foot_desc:"منصة عقارية متميزة تربط المشترين والبائعين والمستثمرين في جميع أنحاء المملكة بعقارات موثقة وعالية القيمة.",
    foot_links:"روابط سريعة", foot_home:"الرئيسية", foot_support:"الدعم",
    foot_help:"مركز المساعدة", foot_terms:"شروط الخدمة", foot_privacy:"سياسة الخصوصية", foot_faq:"الأسئلة الشائعة",
    foot_contact:"تواصل معنا", foot_address:"طريق الملك فهد، الرياض، المملكة العربية السعودية",
    foot_rights:"© 2026 ألباتروس. جميع الحقوق محفوظة.", foot_tagline:"مصمم لسوق العقارات السعودي",

    bc_home:"الرئيسية", bc_properties:"العقارات",
    prop_page_title:"استكشف عقاراتنا",
    prop_page_lead:"تصفح قوائم موثقة في أكثر أحياء المملكة العربية السعودية طلباً.",
    f_title:"تصفية العقارات", f_location:"الموقع", f_all_locations:"كل المواقع",
    f_type:"نوع العقار", f_all_types:"كل الأنواع", f_villa:"فيلا", f_apartment:"شقة", f_townhouse:"تاون هاوس",
    f_status:"الحالة", f_all_status:"الكل", f_forsale:"للبيع", f_forrent:"للإيجار",
    f_price:"نطاق السعر (ريال)", f_min:"الأدنى", f_max:"الأعلى",
    f_beds:"غرف النوم", f_any:"أي عدد",
    f_apply:"تطبيق الفلاتر", f_reset:"إعادة تعيين",
    results_showing:"عرض", results_of:"من", results_properties:"عقار",
    sort_label:"ترتيب حسب", sort_newest:"الأحدث", sort_low:"السعر: من الأقل للأعلى", sort_high:"السعر: من الأعلى للأقل",
    pg_prev:"السابق", pg_next:"التالي",

    d_back:"العودة إلى العقارات",
    d_beds:"غرف النوم", d_baths:"الحمامات", d_area:"المساحة", d_year:"سنة البناء",
    d_desc_h:"الوصف",
    d_desc_p:"يجمع هذا العقار المميز بين العمارة العصرية والراحة الدائمة. يتميز بمساحات معيشة واسعة، وتشطيبات فاخرة، وحديقة خاصة، ليقدم أسلوب حياة راقٍ في واحد من أكثر أحياء الرياض طلباً — على بُعد دقائق من المدارس والمراكز التجارية والأعمال.",
    d_amenities_h:"المرافق",
    am1:"حديقة خاصة", am2:"موقف سيارات مغطى (2)", am3:"نظام منزل ذكي", am4:"تكييف مركزي",
    am5:"غرفة خادمة", am6:"أمن وكاميرات مراقبة", am7:"مسبح", am8:"مصعد",
    d_agent_h:"وكيل العقار", d_agent_name:"فيصل العتيبي", d_agent_role:"مستشار عقاري أول",
    d_request_viewing:"طلب معاينة", d_call:"اتصل بالوكيل",
    d_related_h:"عقارات مشابهة",

    c_eyebrow:"تواصل معنا", c_title:"لنجد عقارك القادم معاً.",
    c_lead:"سواء كنت تشتري أو تبيع أو تستثمر — يرد مستشارونا خلال يوم عمل واحد.",
    c_form_h:"أرسل لنا رسالة",
    c_name:"الاسم الكامل", c_email:"البريد الإلكتروني", c_phone:"رقم الهاتف", c_subject:"أنا مهتم بـ",
    c_subject_buy:"الشراء", c_subject_sell:"البيع", c_subject_rent:"الإيجار", c_subject_other:"أخرى",
    c_message:"الرسالة", c_message_ph:"أخبرنا عمّا تبحث عنه...",
    c_send:"إرسال الرسالة",
    c_info_h:"معلومات المكتب",
    c_office:"المكتب الرئيسي، الرياض", c_hours_h:"ساعات العمل", c_hours:"الأحد – الخميس، 9:00 ص – 6:00 م",
    c_map_note:"الخريطة التفاعلية قريباً",

    /* login.html */
    auth_visual_h:"عقارك القادم على بُعد تسجيل دخول واحد.",
    auth_visual_p:"ادخل إلى عمليات البحث المحفوظة، احجز معاينات، وتواصل مع المستشارين مباشرة من حسابك في ألباتروس.",
    login_h:"مرحباً بعودتك", login_sub:"سجّل الدخول لإدارة عقاراتك ومعايناتك.",
    login_pass:"كلمة المرور", login_remember:"تذكرني", login_forgot:"نسيت كلمة المرور؟",
    login_btn:"تسجيل الدخول", login_or:"أو تابع باستخدام",
    login_noaccount:"ليس لديك حساب؟", login_signup_link:"إنشاء حساب",

    /* register.html */
    reg_visual_h:"انضم لآلاف الباحثين عن عقارهم المثالي.",
    reg_visual_p:"أنشئ حساباً مجانياً لحفظ المفضلة، ومتابعة تغيّر الأسعار، والحصول على أحدث العروض أولاً.",
    reg_h:"إنشاء حسابك", reg_sub:"انضم إلى ألباتروس مجاناً وابدأ استكشاف العقارات.",
    reg_confirm:"تأكيد كلمة المرور", reg_terms:"أوافق على الشروط وسياسة الخصوصية",
    reg_btn:"إنشاء الحساب", reg_hasaccount:"لديك حساب بالفعل؟", reg_login_link:"تسجيل الدخول",
    reg_iam:"أنا", reg_buyer:"مشترٍ", reg_buyer_sub:"أبحث عن شراء أو إيجار",
    reg_seller:"بائع", reg_seller_sub:"أبحث عن عرض عقار للبيع",
    splash_skip:"اضغط للتخطي",

    /* admin.html */
    adm_overview:"نظرة عامة", adm_properties:"العقارات", adm_leads:"العملاء المحتملون", adm_users:"المستخدمون",
    adm_backsite:"العودة للموقع", adm_role:"مدير النظام",
    adm_stat_props:"إجمالي العقارات", adm_stat_active:"قوائم نشطة",
    adm_stat_leads:"عملاء جدد (30 يوم)", adm_stat_users:"المستخدمون المسجلون",
    adm_chart_title:"العقارات المضافة شهرياً", adm_activity_title:"النشاط الأخير",
    adm_search_props:"ابحث في العقارات...", adm_add_property:"إضافة عقار",
    adm_th_property:"العقار", adm_th_type:"النوع", adm_th_price:"السعر", adm_th_status:"الحالة", adm_th_actions:"الإجراءات",
    adm_search_leads:"ابحث في العملاء المحتملين...", adm_all:"الكل", adm_new:"جديد", adm_contacted:"تم التواصل", adm_closed:"مغلق",
    adm_th_name:"الاسم", adm_th_contact:"التواصل", adm_th_interest:"مهتم بـ", adm_th_date:"التاريخ",
    adm_search_users:"ابحث في المستخدمين...", adm_th_user:"المستخدم", adm_th_role:"الدور", adm_th_joined:"تاريخ الانضمام",
    adm_f_title:"العنوان", adm_f_price:"السعر (ريال)", adm_f_listing_status:"حالة الإعلان",
    adm_active:"نشط", adm_pending:"قيد الانتظار", adm_f_image:"رابط الصورة", adm_save:"حفظ العقار",

    /* favorites.html */
    fav_title:"المفضلة", fav_lead:"العقارات اللي حفظتها لوقت لاحق.",
    fav_empty_h:"لا توجد مفضلات بعد", fav_empty_p:"اضغط على أيقونة القلب في أي عقار عشان تحفظه هنا.",
    fav_browse:"تصفح العقارات", nav_favorites:"المفضلة"
  }
};

function getLangFromUrl(){
  const p = new URLSearchParams(window.location.search);
  return p.get('lang') === 'ar' ? 'ar' : 'en';
}

function applyLinkLang(lang){
  document.querySelectorAll('a[href]').forEach(a=>{
    const href = a.getAttribute('href');
    if(!href || href.startsWith('#') || href.startsWith('http') || href.startsWith('mailto') || href.startsWith('tel')) return;
    const [path, query] = href.split('?');
    const params = new URLSearchParams(query || '');
    if(lang === 'ar') params.set('lang','ar'); else params.delete('lang');
    const qs = params.toString();
    a.setAttribute('href', qs ? `${path}?${qs}` : path);
  });
}

function setLang(lang){
  const dict = translations[lang];
  document.querySelectorAll('[data-i18n]').forEach(el=>{
    const key = el.getAttribute('data-i18n');
    if(dict[key] !== undefined) el.textContent = dict[key];
  });
  document.querySelectorAll('[data-i18n-html]').forEach(el=>{
    const key = el.getAttribute('data-i18n-html');
    if(dict[key] !== undefined) el.innerHTML = dict[key];
  });
  document.querySelectorAll('[data-i18n-ph]').forEach(el=>{
    const key = el.getAttribute('data-i18n-ph');
    if(dict[key] !== undefined) el.setAttribute('placeholder', dict[key]);
  });
  document.documentElement.lang = lang;
  document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
  const label = document.getElementById('langLabel');
  if(label) label.textContent = lang === 'ar' ? 'AR' : 'EN';
  applyLinkLang(lang);
  window.__currentLang = lang;
}

document.addEventListener('DOMContentLoaded', ()=>{
  const initial = getLangFromUrl();
  setLang(initial);
  const toggle = document.getElementById('langToggle');
  if(toggle){
    const doToggle = ()=>{
      const next = window.__currentLang === 'ar' ? 'en' : 'ar';
      setLang(next);
    };
    toggle.addEventListener('click', doToggle);
    toggle.addEventListener('keypress', (e)=>{ if(e.key === 'Enter') doToggle(); });
  }

  /* Replay the flying-bird logo animation on click */
  document.querySelectorAll('.logo-mark.bird-logo').forEach(mark=>{
    mark.addEventListener('click', ()=>{
      const g = mark.querySelector('.bird-fly');
      const wings = mark.querySelectorAll('.wing');
      [g, ...wings].forEach(el=>{
        if(!el) return;
        el.style.animation = 'none';
        void el.offsetWidth; /* force reflow to restart animation */
        el.style.animation = '';
      });
    });
  });
});
